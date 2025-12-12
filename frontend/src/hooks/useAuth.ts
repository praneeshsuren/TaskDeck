'use client';

import { useContext, useCallback } from 'react';
import { AuthContext } from '@/contexts/AuthContext';
import {
    signInWithPopup,
    GoogleAuthProvider,
    signOut as firebaseSignOut
} from 'firebase/auth';
import { auth } from '@/lib/firebaseClient';

export function useAuth() {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }

    const signInWithGoogle = useCallback(async () => {
        const provider = new GoogleAuthProvider();
        try {
            const result = await signInWithPopup(auth, provider);
            return result.user;
        } catch (error) {
            console.error('Google sign in error:', error);
            throw error;
        }
    }, []);

    const signOut = useCallback(async () => {
        try {
            await firebaseSignOut(auth);
        } catch (error) {
            console.error('Sign out error:', error);
            throw error;
        }
    }, []);

    return {
        ...context,
        signInWithGoogle,
        signOut,
    };
}

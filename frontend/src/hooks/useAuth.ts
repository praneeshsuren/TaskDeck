'use client';

import { useContext, useCallback } from 'react';
import { AuthContext } from '@/contexts/AuthContext';
import {
    signInWithPopup,
    GoogleAuthProvider,
    signOut as firebaseSignOut
} from 'firebase/auth';
import { auth } from '@/lib/firebaseClient';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

export function useAuth() {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }

    const signInWithGoogle = useCallback(async () => {
        const provider = new GoogleAuthProvider();
        try {
            // Sign in with Firebase using popup
            const result = await signInWithPopup(auth, provider);

            // Get the Firebase ID token
            const idToken = await result.user.getIdToken();

            // Send token to backend to create/update user in database
            const response = await fetch(`${API_BASE_URL}/api/auth/firebase`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ idToken }),
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                console.error('Backend auth error:', errorData);
                throw new Error('Failed to authenticate with backend');
            }

            const authData = await response.json();

            // Store the JWT token from backend
            localStorage.setItem('token', authData.token);
            localStorage.setItem('tokenExpiry', authData.expiresAt);

            return result.user;
        } catch (error) {
            console.error('Google sign in error:', error);
            throw error;
        }
    }, []);

    const signOut = useCallback(async () => {
        try {
            await firebaseSignOut(auth);
            // Clear backend token
            localStorage.removeItem('token');
            localStorage.removeItem('tokenExpiry');
        } catch (error) {
            console.error('Sign out error:', error);
            throw error;
        }
    }, []);

    // Get stored JWT token for API calls
    const getToken = useCallback(() => {
        return localStorage.getItem('token');
    }, []);

    return {
        ...context,
        signInWithGoogle,
        signOut,
        getToken,
    };
}


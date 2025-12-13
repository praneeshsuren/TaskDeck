'use client';

import { createContext, useEffect, useState, ReactNode } from 'react';
import { User, onAuthStateChanged, getRedirectResult } from 'firebase/auth';
import { auth } from '@/lib/firebaseClient';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

interface AuthUser {
    id: string;
    email: string | null;
    displayName: string | null;
    avatarUrl: string | null;
}

interface AuthContextType {
    user: AuthUser | null;
    loading: boolean;
    firebaseUser: User | null;
    authError: string | null;
}

export const AuthContext = createContext<AuthContextType>({
    user: null,
    loading: true,
    firebaseUser: null,
    authError: null,
});

interface AuthProviderProps {
    children: ReactNode;
}

async function authenticateWithBackend(firebaseUser: User) {
    try {
        // Get the Firebase ID token
        const idToken = await firebaseUser.getIdToken();

        // Send token to backend to create/update user in database
        const response = await fetch(`${API_BASE_URL}/api/auth/firebase`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ idToken }),
        });

        if (!response.ok) {
            throw new Error('Failed to authenticate with backend');
        }

        const authData = await response.json();

        // Store the JWT token from backend
        localStorage.setItem('token', authData.token);
        localStorage.setItem('tokenExpiry', authData.expiresAt);
        
        return true;
    } catch (error) {
        console.error('Backend authentication error:', error);
        throw error;
    }
}

export function AuthProvider({ children }: AuthProviderProps) {
    const [user, setUser] = useState<AuthUser | null>(null);
    const [firebaseUser, setFirebaseUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);
    const [authError, setAuthError] = useState<string | null>(null);

    // Handle redirect result on mount
    useEffect(() => {
        const handleRedirectResult = async () => {
            try {
                const result = await getRedirectResult(auth);
                if (result?.user) {
                    // User just signed in via redirect, authenticate with backend
                    await authenticateWithBackend(result.user);
                    // Redirect to dashboard after successful sign-in
                    window.location.href = '/dashboard';
                }
            } catch (error) {
                console.error('Error handling redirect result:', error);
                setAuthError('Failed to complete sign-in. Please try again.');
            }
        };

        handleRedirectResult();
    }, []);

    useEffect(() => {
        const unsubscribe = onAuthStateChanged(auth, async (firebaseUser) => {
            if (firebaseUser) {
                setFirebaseUser(firebaseUser);
                setUser({
                    id: firebaseUser.uid,
                    email: firebaseUser.email,
                    displayName: firebaseUser.displayName,
                    avatarUrl: firebaseUser.photoURL,
                });
                
                // If we don't have a token, get one from the backend
                const existingToken = localStorage.getItem('token');
                if (!existingToken) {
                    try {
                        await authenticateWithBackend(firebaseUser);
                    } catch (error) {
                        console.error('Failed to authenticate with backend:', error);
                        setAuthError('Failed to authenticate with server.');
                    }
                }
            } else {
                setFirebaseUser(null);
                setUser(null);
            }
            setLoading(false);
        });

        return () => unsubscribe();
    }, []);

    return (
        <AuthContext.Provider value={{ user, loading, firebaseUser, authError }}>
            {children}
        </AuthContext.Provider>
    );
}

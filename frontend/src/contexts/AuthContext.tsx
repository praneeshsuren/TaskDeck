'use client';

import { createContext, useEffect, useState, ReactNode } from 'react';
import { User, onAuthStateChanged } from 'firebase/auth';
import { auth } from '@/lib/firebaseClient';

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
}

export const AuthContext = createContext<AuthContextType>({
    user: null,
    loading: true,
    firebaseUser: null,
});

interface AuthProviderProps {
    children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
    const [user, setUser] = useState<AuthUser | null>(null);
    const [firebaseUser, setFirebaseUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const unsubscribe = onAuthStateChanged(auth, (firebaseUser) => {
            if (firebaseUser) {
                setFirebaseUser(firebaseUser);
                setUser({
                    id: firebaseUser.uid,
                    email: firebaseUser.email,
                    displayName: firebaseUser.displayName,
                    avatarUrl: firebaseUser.photoURL,
                });
            } else {
                setFirebaseUser(null);
                setUser(null);
            }
            setLoading(false);
        });

        return () => unsubscribe();
    }, []);

    return (
        <AuthContext.Provider value={{ user, loading, firebaseUser }}>
            {children}
        </AuthContext.Provider>
    );
}

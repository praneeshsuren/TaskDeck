'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';

export default function AuthCallbackPage() {
    const router = useRouter();
    const { signInWithGoogle, loading } = useAuth();

    useEffect(() => {
        const handleAuth = async () => {
            try {
                await signInWithGoogle();
                router.push('/dashboard');
            } catch (error) {
                console.error('Auth error:', error);
                router.push('/');
            }
        };

        if (!loading) {
            handleAuth();
        }
    }, [loading, router, signInWithGoogle]);

    return (
        <div className="flex items-center justify-center min-h-[calc(100vh-64px)]">
            <div className="text-center">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500 mx-auto mb-4" />
                <p className="text-gray-600 dark:text-gray-400">Signing you in...</p>
            </div>
        </div>
    );
}

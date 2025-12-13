'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';

export default function AuthCallbackPage() {
    const router = useRouter();
    const { user, signInWithGoogle, loading } = useAuth();
    const [isSigningIn, setIsSigningIn] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Redirect to dashboard if already logged in
    useEffect(() => {
        if (!loading && user) {
            router.push('/dashboard');
        }
    }, [loading, user, router]);

    const handleSignIn = async () => {
        setIsSigningIn(true);
        setError(null);
        try {
            await signInWithGoogle();
            // Navigate to dashboard after successful sign-in
            router.push('/dashboard');
        } catch (err) {
            console.error('Auth error:', err);
            setError('Failed to sign in. Please try again.');
            setIsSigningIn(false);
        }
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[calc(100vh-64px)]">
                <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500 mx-auto mb-4" />
                    <p className="text-gray-600 dark:text-gray-400">Loading...</p>
                </div>
            </div>
        );
    }

    if (user) {
        return (
            <div className="flex items-center justify-center min-h-[calc(100vh-64px)]">
                <div className="text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500 mx-auto mb-4" />
                    <p className="text-gray-600 dark:text-gray-400">Redirecting to dashboard...</p>
                </div>
            </div>
        );
    }

    return (
        <div className="flex items-center justify-center min-h-[calc(100vh-64px)]">
            <div className="text-center max-w-md mx-auto px-4">
                <h1 className="text-3xl font-bold text-gray-900 dark:text-white mb-4">
                    Welcome to TaskDeck
                </h1>
                <p className="text-gray-600 dark:text-gray-400 mb-8">
                    Sign in with your Google account to get started
                </p>

                {error && (
                    <div className="mb-4 p-3 bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-400 rounded-lg text-sm">
                        {error}
                    </div>
                )}

                <button
                    onClick={handleSignIn}
                    disabled={isSigningIn}
                    className="w-full flex items-center justify-center gap-3 px-6 py-3 bg-white dark:bg-dark-card border border-gray-300 dark:border-dark-border rounded-lg shadow-sm hover:shadow-md transition-all duration-200 text-gray-700 dark:text-white font-medium disabled:opacity-50 disabled:cursor-not-allowed"
                >
                    {isSigningIn ? (
                        <>
                            <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-primary-500" />
                            <span>Signing in...</span>
                        </>
                    ) : (
                        <>
                            <svg className="w-5 h-5" viewBox="0 0 24 24">
                                <path
                                    fill="#4285F4"
                                    d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
                                />
                                <path
                                    fill="#34A853"
                                    d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                                />
                                <path
                                    fill="#FBBC05"
                                    d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                                />
                                <path
                                    fill="#EA4335"
                                    d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                                />
                            </svg>
                            <span>Continue with Google</span>
                        </>
                    )}
                </button>

                <p className="mt-6 text-sm text-gray-500 dark:text-gray-400">
                    By signing in, you agree to our Terms of Service and Privacy Policy
                </p>
            </div>
        </div>
    );
}

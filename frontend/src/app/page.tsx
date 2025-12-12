'use client';

import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';
import { CheckSquare, ArrowRight, Zap, Users, Shield } from 'lucide-react';

export default function HomePage() {
    const { user, loading } = useAuth();

    return (
        <div className="min-h-[calc(100vh-64px)]">
            {/* Hero Section */}
            <section className="relative overflow-hidden">
                <div className="absolute inset-0 bg-gradient-to-br from-primary-500/10 via-transparent to-purple-500/10" />
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24 relative">
                    <div className="text-center">
                        <h1 className="text-5xl md:text-6xl font-bold text-gray-900 dark:text-white mb-6">
                            Manage Tasks with
                            <span className="text-transparent bg-clip-text bg-gradient-to-r from-primary-500 to-purple-500">
                                {' '}
                                TaskDeck
                            </span>
                        </h1>
                        <p className="text-xl text-gray-600 dark:text-gray-300 max-w-2xl mx-auto mb-8">
                            A modern, real-time task management platform that helps teams collaborate
                            effectively and ship projects faster.
                        </p>
                        <div className="flex flex-col sm:flex-row gap-4 justify-center">
                            {!loading && !user ? (
                                <>
                                    <Link
                                        href="/auth/callback"
                                        className="btn-primary inline-flex items-center justify-center gap-2 text-lg px-8 py-3"
                                    >
                                        Get Started <ArrowRight className="w-5 h-5" />
                                    </Link>
                                    <Link
                                        href="/dashboard"
                                        className="btn-secondary inline-flex items-center justify-center gap-2 text-lg px-8 py-3"
                                    >
                                        View Demo
                                    </Link>
                                </>
                            ) : (
                                <Link
                                    href="/dashboard"
                                    className="btn-primary inline-flex items-center justify-center gap-2 text-lg px-8 py-3"
                                >
                                    Go to Dashboard <ArrowRight className="w-5 h-5" />
                                </Link>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {/* Features Section */}
            <section className="py-24 bg-white dark:bg-dark-card">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <h2 className="text-3xl font-bold text-center text-gray-900 dark:text-white mb-12">
                        Everything you need to manage tasks
                    </h2>
                    <div className="grid md:grid-cols-3 gap-8">
                        <FeatureCard
                            icon={<Zap className="w-8 h-8 text-primary-500" />}
                            title="Real-time Updates"
                            description="See changes instantly with SignalR-powered real-time synchronization across all team members."
                        />
                        <FeatureCard
                            icon={<Users className="w-8 h-8 text-primary-500" />}
                            title="Team Collaboration"
                            description="Assign tasks, track progress, and collaborate seamlessly with your team members."
                        />
                        <FeatureCard
                            icon={<Shield className="w-8 h-8 text-primary-500" />}
                            title="Secure & Fast"
                            description="Enterprise-grade security with Firebase authentication and optimized performance."
                        />
                    </div>
                </div>
            </section>
        </div>
    );
}

function FeatureCard({
    icon,
    title,
    description,
}: {
    icon: React.ReactNode;
    title: string;
    description: string;
}) {
    return (
        <div className="card p-6 hover:shadow-lg transition-shadow duration-300">
            <div className="mb-4">{icon}</div>
            <h3 className="text-xl font-semibold text-gray-900 dark:text-white mb-2">{title}</h3>
            <p className="text-gray-600 dark:text-gray-400">{description}</p>
        </div>
    );
}

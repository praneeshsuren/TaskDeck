'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/hooks/useAuth';
import { ToggleDarkMode } from '@/components/ui/ToggleDarkMode';
import { NotificationsDropdown } from '@/components/NotificationsDropdown';
import { CheckSquare, Menu, X, LogOut } from 'lucide-react';
import { useState } from 'react';

export function Navbar() {
    const pathname = usePathname();
    const { user, signOut } = useAuth();
    const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

    const navLinks = [
        { href: '/dashboard', label: 'Dashboard' },
        { href: '/tasks', label: 'Tasks' },
    ];

    return (
        <nav className="sticky top-0 z-50 bg-white/80 dark:bg-dark-bg/80 backdrop-blur-lg border-b border-gray-200 dark:border-dark-border">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex items-center justify-between h-16">
                    {/* Logo */}
                    <Link href="/" className="flex items-center gap-2">
                        <CheckSquare className="w-8 h-8 text-primary-500" />
                        <span className="text-xl font-bold text-gray-900 dark:text-white">
                            TaskDeck
                        </span>
                    </Link>

                    {/* Desktop Navigation */}
                    <div className="hidden md:flex items-center gap-6">
                        {user && navLinks.map((link) => (
                            <Link
                                key={link.href}
                                href={link.href}
                                className={`text-sm font-medium transition-colors ${pathname === link.href
                                        ? 'text-primary-600 dark:text-primary-400'
                                        : 'text-gray-600 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white'
                                    }`}
                            >
                                {link.label}
                            </Link>
                        ))}
                    </div>

                    {/* Right Side */}
                    <div className="flex items-center gap-4">
                        <ToggleDarkMode />

                        {user && <NotificationsDropdown />}

                        {user ? (
                            <div className="hidden md:flex items-center gap-4">
                                <div className="flex items-center gap-2">
                                    {user.avatarUrl && (
                                        <img
                                            src={user.avatarUrl}
                                            alt={user.displayName || 'User'}
                                            className="w-8 h-8 rounded-full"
                                        />
                                    )}
                                    <span className="text-sm font-medium text-gray-700 dark:text-gray-300">
                                        {user.displayName}
                                    </span>
                                </div>
                                <button
                                    onClick={signOut}
                                    className="p-2 text-gray-500 hover:text-gray-700 dark:hover:text-gray-300"
                                >
                                    <LogOut className="w-5 h-5" />
                                </button>
                            </div>
                        ) : (
                            <Link href="/auth/callback" className="btn-primary text-sm">
                                Sign In
                            </Link>
                        )}

                        {/* Mobile menu button */}
                        <button
                            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
                            className="md:hidden p-2 text-gray-500"
                        >
                            {mobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
                        </button>
                    </div>
                </div>

                {/* Mobile Navigation */}
                {mobileMenuOpen && (
                    <div className="md:hidden py-4 border-t border-gray-200 dark:border-dark-border">
                        {user && navLinks.map((link) => (
                            <Link
                                key={link.href}
                                href={link.href}
                                onClick={() => setMobileMenuOpen(false)}
                                className={`block py-2 text-sm font-medium ${pathname === link.href
                                        ? 'text-primary-600 dark:text-primary-400'
                                        : 'text-gray-600 dark:text-gray-300'
                                    }`}
                            >
                                {link.label}
                            </Link>
                        ))}
                        {user && (
                            <button
                                onClick={() => {
                                    signOut();
                                    setMobileMenuOpen(false);
                                }}
                                className="w-full text-left py-2 text-sm font-medium text-red-600"
                            >
                                Sign Out
                            </button>
                        )}
                    </div>
                )}
            </div>
        </nav>
    );
}

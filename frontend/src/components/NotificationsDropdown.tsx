'use client';

import { useState, useEffect, useRef } from 'react';
import { Bell, Check, X } from 'lucide-react';
import { api } from "@/lib/apiClient"; 
import { Invitation } from '@/types/api';

export function NotificationsDropdown() {
    const [invitations, setInvitations] = useState<Invitation[]>([]);
    const [isOpen, setIsOpen] = useState(false);
    const [loading, setLoading] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const fetchInvitations = async () => {
        try {
            const response = await api.getInvitations();
            setInvitations(response.data);
        } catch (error) {
            console.error('Failed to fetch invitations:', error);
        }
    };

    useEffect(() => {
        fetchInvitations();
        // Poll for new invitations every 30 seconds
        const interval = setInterval(fetchInvitations, 30000);
        return () => clearInterval(interval);
    }, []);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleAccept = async (invitationId: string) => {
        setLoading(true);
        try {
            await api.acceptInvitation(invitationId);
            setInvitations(prev => prev.filter(inv => inv.id !== invitationId));
            // Trigger a page refresh to update the sidebar
            window.dispatchEvent(new CustomEvent('invitationAccepted'));
        } catch (error) {
            console.error('Failed to accept invitation:', error);
        }
        setLoading(false);
    };

    const handleDecline = async (invitationId: string) => {
        setLoading(true);
        try {
            await api.declineInvitation(invitationId);
            setInvitations(prev => prev.filter(inv => inv.id !== invitationId));
        } catch (error) {
            console.error('Failed to decline invitation:', error);
        }
        setLoading(false);
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="relative p-2 text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 transition-colors"
            >
                <Bell className="w-5 h-5" />
                {invitations.length > 0 && (
                    <span className="absolute top-1 right-1 w-4 h-4 bg-red-500 text-white text-xs font-medium rounded-full flex items-center justify-center">
                        {invitations.length}
                    </span>
                )}
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-2 w-80 bg-white dark:bg-dark-card rounded-lg shadow-lg border border-gray-200 dark:border-dark-border z-50">
                    <div className="p-3 border-b border-gray-200 dark:border-dark-border">
                        <h3 className="font-semibold text-gray-900 dark:text-white">
                            Notifications
                        </h3>
                    </div>

                    <div className="max-h-80 overflow-y-auto">
                        {invitations.length === 0 ? (
                            <div className="p-4 text-center text-gray-500 dark:text-gray-400">
                                No pending invitations
                            </div>
                        ) : (
                            invitations.map((invitation) => (
                                <div
                                    key={invitation.id}
                                    className="p-3 border-b border-gray-100 dark:border-dark-border last:border-0"
                                >
                                    <div className="flex items-start gap-3">
                                        <div
                                            className="w-3 h-3 rounded-full mt-1.5 flex-shrink-0"
                                            style={{ backgroundColor: invitation.projectColor || '#3b82f6' }}
                                        />
                                        <div className="flex-1 min-w-0">
                                            <p className="text-sm text-gray-900 dark:text-white">
                                                <span className="font-medium">{invitation.invitedBy.displayName}</span>
                                                {' invited you to join '}
                                                <span className="font-medium">{invitation.projectName}</span>
                                            </p>
                                            <div className="mt-2 flex gap-2">
                                                <button
                                                    onClick={() => handleAccept(invitation.id)}
                                                    disabled={loading}
                                                    className="flex items-center gap-1 px-3 py-1 text-xs font-medium text-white bg-green-600 hover:bg-green-700 rounded-md transition-colors disabled:opacity-50"
                                                >
                                                    <Check className="w-3 h-3" />
                                                    Accept
                                                </button>
                                                <button
                                                    onClick={() => handleDecline(invitation.id)}
                                                    disabled={loading}
                                                    className="flex items-center gap-1 px-3 py-1 text-xs font-medium text-gray-700 dark:text-gray-200 bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 rounded-md transition-colors disabled:opacity-50"
                                                >
                                                    <X className="w-3 h-3" />
                                                    Decline
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>
            )}
        </div>
    );
}

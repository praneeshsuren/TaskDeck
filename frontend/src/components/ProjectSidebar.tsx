'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { FolderOpen, Plus, ChevronRight, X, Users } from 'lucide-react';
import { api } from '@/lib/apiClient';
import type { Project } from '@/types/todo';

interface ProjectSidebarProps {
    projects: Project[];
    onProjectCreated?: () => void;
}

export function ProjectSidebar({ projects, onProjectCreated }: ProjectSidebarProps) {
    const pathname = usePathname();
    const router = useRouter();
    const [isCreating, setIsCreating] = useState(false);
    const [newProjectName, setNewProjectName] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    // Listen for invitation accepted events to refresh projects
    useEffect(() => {
        const handleInvitationAccepted = () => {
            onProjectCreated?.();
        };

        window.addEventListener('invitationAccepted', handleInvitationAccepted);
        return () => window.removeEventListener('invitationAccepted', handleInvitationAccepted);
    }, [onProjectCreated]);

    const handleCreateProject = async () => {
        if (!newProjectName.trim()) return;

        setIsLoading(true);
        try {
            const response = await api.createProject({
                name: newProjectName.trim(),
                color: '#3b82f6',
                icon: 'folder'
            });
            
            setNewProjectName('');
            setIsCreating(false);
            onProjectCreated?.();
            
            // Navigate to the new project
            router.push(`/projects/${response.data.id}`);
        } catch (error) {
            console.error('Failed to create project:', error);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <aside className="w-64 bg-white dark:bg-dark-card border-r border-gray-200 dark:border-dark-border hidden lg:block overflow-y-auto">
            <div className="p-4">
                <div className="flex items-center justify-between mb-4">
                    <h2 className="text-sm font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                        Projects
                    </h2>
                    <button 
                        onClick={() => setIsCreating(true)}
                        className="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 rounded"
                    >
                        <Plus className="w-5 h-5" />
                    </button>
                </div>

                {/* Create Project Form */}
                {isCreating && (
                    <div className="mb-4 p-3 bg-gray-50 dark:bg-dark-border rounded-lg">
                        <input
                            type="text"
                            value={newProjectName}
                            onChange={(e) => setNewProjectName(e.target.value)}
                            onKeyDown={(e) => e.key === 'Enter' && handleCreateProject()}
                            placeholder="Project name"
                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-dark-card focus:outline-none focus:ring-2 focus:ring-primary-500"
                            autoFocus
                        />
                        <div className="flex gap-2 mt-2">
                            <button
                                onClick={handleCreateProject}
                                disabled={isLoading || !newProjectName.trim()}
                                className="flex-1 px-3 py-1.5 text-sm bg-primary-600 text-white rounded-md hover:bg-primary-700 disabled:opacity-50"
                            >
                                {isLoading ? 'Creating...' : 'Create'}
                            </button>
                            <button
                                onClick={() => {
                                    setIsCreating(false);
                                    setNewProjectName('');
                                }}
                                className="p-1.5 text-gray-400 hover:text-gray-600 rounded-md"
                            >
                                <X className="w-4 h-4" />
                            </button>
                        </div>
                    </div>
                )}

                <nav className="space-y-1">
                    {projects.length === 0 ? (
                        <p className="text-sm text-gray-500 dark:text-gray-400 py-4 text-center">
                            No projects yet
                        </p>
                    ) : (
                        projects.map((project) => (
                            <Link
                                key={project.id}
                                href={`/projects/${project.id}`}
                                className={`flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-colors ${pathname === `/projects/${project.id}`
                                        ? 'bg-primary-50 dark:bg-primary-900/20 text-primary-600 dark:text-primary-400'
                                        : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-dark-border'
                                    }`}
                            >
                                <div
                                    className="w-3 h-3 rounded-full flex-shrink-0"
                                    style={{ backgroundColor: project.color || '#3b82f6' }}
                                />
                                <span className="truncate flex-1">{project.name}</span>
                                {!project.isOwner && (
                                    <span title="Shared project">
                                        <Users className="w-3.5 h-3.5 text-gray-400" />
                                    </span>
                                )}
                                <ChevronRight className="w-4 h-4 text-gray-400" />
                            </Link>
                        ))
                    )}
                </nav>
            </div>

            {/* Quick Links */}
            <div className="p-4 border-t border-gray-200 dark:border-dark-border">
                <h2 className="text-sm font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider mb-4">
                    Quick Links
                </h2>
                <nav className="space-y-1">
                    <Link
                        href="/tasks"
                        className="flex items-center gap-3 px-3 py-2 rounded-lg text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-dark-border"
                    >
                        <FolderOpen className="w-5 h-5 text-gray-400" />
                        All Tasks
                    </Link>
                </nav>
            </div>
        </aside>
    );
}

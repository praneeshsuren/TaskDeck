'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { FolderOpen, Plus, ChevronRight } from 'lucide-react';
import type { Project } from '@/types/todo';

interface ProjectSidebarProps {
    projects: Project[];
}

export function ProjectSidebar({ projects }: ProjectSidebarProps) {
    const pathname = usePathname();

    return (
        <aside className="w-64 bg-white dark:bg-dark-card border-r border-gray-200 dark:border-dark-border hidden lg:block overflow-y-auto">
            <div className="p-4">
                <div className="flex items-center justify-between mb-4">
                    <h2 className="text-sm font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                        Projects
                    </h2>
                    <button className="p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 rounded">
                        <Plus className="w-5 h-5" />
                    </button>
                </div>

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

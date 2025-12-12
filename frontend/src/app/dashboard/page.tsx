'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import { useAuth } from '@/hooks/useAuth';
import { ProjectSidebar } from '@/components/ProjectSidebar';
import { TaskCard } from '@/components/TaskCard';
import { Plus, CheckCircle2, Clock, AlertCircle } from 'lucide-react';
import type { Task, Project } from '@/types/todo';

export default function DashboardPage() {
    const { user } = useAuth();
    const [projects, setProjects] = useState<Project[]>([]);
    const [recentTasks, setRecentTasks] = useState<Task[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // TODO: Fetch projects and tasks from API
        setLoading(false);
    }, []);

    const stats = {
        total: recentTasks.length,
        completed: recentTasks.filter((t) => t.status === 'Done').length,
        inProgress: recentTasks.filter((t) => t.status === 'InProgress').length,
        overdue: recentTasks.filter(
            (t) => t.dueDate && new Date(t.dueDate) < new Date() && t.status !== 'Done'
        ).length,
    };

    return (
        <div className="flex h-[calc(100vh-64px)]">
            <ProjectSidebar projects={projects} />

            <div className="flex-1 overflow-auto p-6">
                <div className="max-w-6xl mx-auto">
                    {/* Header */}
                    <div className="flex items-center justify-between mb-8">
                        <div>
                            <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                                Welcome back{user?.displayName ? `, ${user.displayName}` : ''}
                            </h1>
                            <p className="text-gray-600 dark:text-gray-400 mt-1">
                                Here's what's happening with your tasks today.
                            </p>
                        </div>
                        <Link href="/tasks/new" className="btn-primary inline-flex items-center gap-2">
                            <Plus className="w-5 h-5" />
                            New Task
                        </Link>
                    </div>

                    {/* Stats Grid */}
                    <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
                        <StatCard
                            icon={<CheckCircle2 className="w-6 h-6 text-green-500" />}
                            label="Completed"
                            value={stats.completed}
                            bgColor="bg-green-50 dark:bg-green-900/20"
                        />
                        <StatCard
                            icon={<Clock className="w-6 h-6 text-blue-500" />}
                            label="In Progress"
                            value={stats.inProgress}
                            bgColor="bg-blue-50 dark:bg-blue-900/20"
                        />
                        <StatCard
                            icon={<AlertCircle className="w-6 h-6 text-red-500" />}
                            label="Overdue"
                            value={stats.overdue}
                            bgColor="bg-red-50 dark:bg-red-900/20"
                        />
                        <StatCard
                            icon={<CheckCircle2 className="w-6 h-6 text-gray-500" />}
                            label="Total Tasks"
                            value={stats.total}
                            bgColor="bg-gray-50 dark:bg-gray-800"
                        />
                    </div>

                    {/* Recent Tasks */}
                    <div className="card p-6">
                        <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
                            Recent Tasks
                        </h2>
                        {loading ? (
                            <div className="flex justify-center py-8">
                                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-500" />
                            </div>
                        ) : recentTasks.length === 0 ? (
                            <div className="text-center py-8">
                                <p className="text-gray-500 dark:text-gray-400 mb-4">
                                    No tasks yet. Create your first task to get started!
                                </p>
                                <Link href="/tasks/new" className="btn-primary inline-flex items-center gap-2">
                                    <Plus className="w-5 h-5" />
                                    Create Task
                                </Link>
                            </div>
                        ) : (
                            <div className="space-y-3">
                                {recentTasks.map((task) => (
                                    <TaskCard key={task.id} task={task} />
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

function StatCard({
    icon,
    label,
    value,
    bgColor,
}: {
    icon: React.ReactNode;
    label: string;
    value: number;
    bgColor: string;
}) {
    return (
        <div className={`card p-4 ${bgColor}`}>
            <div className="flex items-center gap-3">
                {icon}
                <div>
                    <p className="text-2xl font-bold text-gray-900 dark:text-white">{value}</p>
                    <p className="text-sm text-gray-600 dark:text-gray-400">{label}</p>
                </div>
            </div>
        </div>
    );
}

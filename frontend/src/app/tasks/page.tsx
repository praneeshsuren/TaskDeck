'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import { TaskCard } from '@/components/TaskCard';
import { Plus, Search, Filter } from 'lucide-react';
import type { Task } from '@/types/todo';

export default function TasksPage() {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState('');
    const [statusFilter, setStatusFilter] = useState<string>('all');

    useEffect(() => {
        // TODO: Fetch tasks from API
        setLoading(false);
    }, []);

    const filteredTasks = tasks.filter((task) => {
        const matchesSearch = task.title.toLowerCase().includes(searchQuery.toLowerCase());
        const matchesStatus = statusFilter === 'all' || task.status === statusFilter;
        return matchesSearch && matchesStatus;
    });

    const groupedTasks = {
        Todo: filteredTasks.filter((t) => t.status === 'Todo'),
        InProgress: filteredTasks.filter((t) => t.status === 'InProgress'),
        InReview: filteredTasks.filter((t) => t.status === 'InReview'),
        Done: filteredTasks.filter((t) => t.status === 'Done'),
    };

    return (
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            {/* Header */}
            <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-8">
                <h1 className="text-3xl font-bold text-gray-900 dark:text-white">Tasks</h1>
                <Link href="/tasks/new" className="btn-primary inline-flex items-center gap-2">
                    <Plus className="w-5 h-5" />
                    New Task
                </Link>
            </div>

            {/* Filters */}
            <div className="card p-4 mb-6">
                <div className="flex flex-col sm:flex-row gap-4">
                    <div className="relative flex-1">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                        <input
                            type="text"
                            placeholder="Search tasks..."
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="input pl-10"
                        />
                    </div>
                    <div className="flex items-center gap-2">
                        <Filter className="w-5 h-5 text-gray-400" />
                        <select
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value)}
                            className="input w-auto"
                        >
                            <option value="all">All Status</option>
                            <option value="Todo">To Do</option>
                            <option value="InProgress">In Progress</option>
                            <option value="InReview">In Review</option>
                            <option value="Done">Done</option>
                        </select>
                    </div>
                </div>
            </div>

            {/* Task Columns (Kanban style) */}
            {loading ? (
                <div className="flex justify-center py-12">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500" />
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                    {Object.entries(groupedTasks).map(([status, statusTasks]) => (
                        <div key={status} className="space-y-3">
                            <div className="flex items-center justify-between">
                                <h2 className="font-semibold text-gray-900 dark:text-white">
                                    {status.replace(/([A-Z])/g, ' $1').trim()}
                                </h2>
                                <span className="text-sm text-gray-500 dark:text-gray-400">
                                    {statusTasks.length}
                                </span>
                            </div>
                            <div className="space-y-3 min-h-[200px] p-2 bg-gray-100 dark:bg-dark-card rounded-lg">
                                {statusTasks.length === 0 ? (
                                    <p className="text-center text-gray-400 text-sm py-4">No tasks</p>
                                ) : (
                                    statusTasks.map((task) => <TaskCard key={task.id} task={task} />)
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

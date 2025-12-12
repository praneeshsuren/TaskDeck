'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { TaskForm } from '@/components/TaskForm';
import { ArrowLeft, Trash2 } from 'lucide-react';
import type { Task } from '@/types/todo';

export default function TaskDetailPage() {
    const params = useParams();
    const router = useRouter();
    const [task, setTask] = useState<Task | null>(null);
    const [loading, setLoading] = useState(true);
    const [isEditing, setIsEditing] = useState(false);

    useEffect(() => {
        // TODO: Fetch task from API using params.id
        setLoading(false);
    }, [params.id]);

    const handleDelete = async () => {
        if (!confirm('Are you sure you want to delete this task?')) return;

        // TODO: Delete task via API
        router.push('/tasks');
    };

    if (loading) {
        return (
            <div className="flex justify-center py-12">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-500" />
            </div>
        );
    }

    if (!task) {
        return (
            <div className="max-w-2xl mx-auto px-4 py-8 text-center">
                <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                    Task not found
                </h1>
                <Link href="/tasks" className="text-primary-500 hover:underline">
                    Back to tasks
                </Link>
            </div>
        );
    }

    return (
        <div className="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            {/* Header */}
            <div className="flex items-center justify-between mb-6">
                <Link
                    href="/tasks"
                    className="inline-flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white"
                >
                    <ArrowLeft className="w-5 h-5" />
                    Back to tasks
                </Link>
                <div className="flex items-center gap-2">
                    <button
                        onClick={() => setIsEditing(!isEditing)}
                        className="btn-secondary"
                    >
                        {isEditing ? 'Cancel' : 'Edit'}
                    </button>
                    <button
                        onClick={handleDelete}
                        className="p-2 text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg transition-colors"
                    >
                        <Trash2 className="w-5 h-5" />
                    </button>
                </div>
            </div>

            {/* Task Content */}
            <div className="card p-6">
                {isEditing ? (
                    <TaskForm
                        task={task}
                        onSuccess={() => {
                            setIsEditing(false);
                            // Refresh task data
                        }}
                    />
                ) : (
                    <div>
                        <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                            {task.title}
                        </h1>
                        {task.description && (
                            <p className="text-gray-600 dark:text-gray-400 mb-6">{task.description}</p>
                        )}
                        <div className="grid grid-cols-2 gap-4 text-sm">
                            <div>
                                <span className="text-gray-500">Status</span>
                                <p className="font-medium text-gray-900 dark:text-white">{task.status}</p>
                            </div>
                            <div>
                                <span className="text-gray-500">Priority</span>
                                <p className="font-medium text-gray-900 dark:text-white">{task.priority}</p>
                            </div>
                            {task.dueDate && (
                                <div>
                                    <span className="text-gray-500">Due Date</span>
                                    <p className="font-medium text-gray-900 dark:text-white">
                                        {new Date(task.dueDate).toLocaleDateString()}
                                    </p>
                                </div>
                            )}
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}

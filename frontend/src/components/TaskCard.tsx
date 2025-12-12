'use client';

import Link from 'next/link';
import { Clock, User, AlertCircle } from 'lucide-react';
import { formatDistanceToNow } from '@/utils/date';
import type { Task } from '@/types/todo';

interface TaskCardProps {
    task: Task;
    compact?: boolean;
}

const priorityColors = {
    Low: 'bg-gray-100 text-gray-600 dark:bg-gray-800 dark:text-gray-400',
    Medium: 'bg-blue-100 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400',
    High: 'bg-orange-100 text-orange-600 dark:bg-orange-900/30 dark:text-orange-400',
    Urgent: 'bg-red-100 text-red-600 dark:bg-red-900/30 dark:text-red-400',
};

const statusColors = {
    Todo: 'border-l-gray-400',
    InProgress: 'border-l-blue-500',
    InReview: 'border-l-purple-500',
    Done: 'border-l-green-500',
    Cancelled: 'border-l-red-500',
};

export function TaskCard({ task, compact = false }: TaskCardProps) {
    const isOverdue =
        task.dueDate && new Date(task.dueDate) < new Date() && task.status !== 'Done';

    return (
        <Link href={`/tasks/${task.id}`}>
            <div
                className={`card p-4 border-l-4 ${statusColors[task.status]} hover:shadow-md transition-all duration-200 animate-fade-in`}
            >
                <div className="flex items-start justify-between gap-3">
                    <div className="flex-1 min-w-0">
                        <h3 className="font-medium text-gray-900 dark:text-white truncate">
                            {task.title}
                        </h3>
                        {!compact && task.description && (
                            <p className="text-sm text-gray-500 dark:text-gray-400 mt-1 line-clamp-2">
                                {task.description}
                            </p>
                        )}
                    </div>
                    <span
                        className={`px-2 py-1 text-xs font-medium rounded-full ${priorityColors[task.priority]}`}
                    >
                        {task.priority}
                    </span>
                </div>

                <div className="flex items-center gap-4 mt-3 text-sm text-gray-500 dark:text-gray-400">
                    {task.dueDate && (
                        <div className={`flex items-center gap-1 ${isOverdue ? 'text-red-500' : ''}`}>
                            {isOverdue ? (
                                <AlertCircle className="w-4 h-4" />
                            ) : (
                                <Clock className="w-4 h-4" />
                            )}
                            <span>{formatDistanceToNow(new Date(task.dueDate))}</span>
                        </div>
                    )}
                    {task.assignedTo && (
                        <div className="flex items-center gap-1">
                            {task.assignedTo.avatarUrl ? (
                                <img
                                    src={task.assignedTo.avatarUrl}
                                    alt={task.assignedTo.displayName}
                                    className="w-5 h-5 rounded-full"
                                />
                            ) : (
                                <User className="w-4 h-4" />
                            )}
                            <span className="truncate max-w-[100px]">{task.assignedTo.displayName}</span>
                        </div>
                    )}
                </div>
            </div>
        </Link>
    );
}

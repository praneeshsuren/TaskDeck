'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import type { Task, TaskPriority, TaskStatus } from '@/types/todo';

interface TaskFormProps {
    task?: Task;
    onSuccess?: () => void;
}

interface FormData {
    title: string;
    description: string;
    priority: TaskPriority;
    status: TaskStatus;
    dueDate: string;
}

export function TaskForm({ task, onSuccess }: TaskFormProps) {
    const router = useRouter();
    const [loading, setLoading] = useState(false);
    const [formData, setFormData] = useState<FormData>({
        title: task?.title || '',
        description: task?.description || '',
        priority: task?.priority || 'Medium',
        status: task?.status || 'Todo',
        dueDate: task?.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : '',
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        try {
            // TODO: Submit to API
            console.log('Submitting:', formData);

            if (onSuccess) {
                onSuccess();
            } else {
                router.push('/tasks');
            }
        } catch (error) {
            console.error('Error saving task:', error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-6">
            {/* Title */}
            <div>
                <label htmlFor="title" className="label">
                    Title <span className="text-red-500">*</span>
                </label>
                <input
                    type="text"
                    id="title"
                    value={formData.title}
                    onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                    className="input"
                    placeholder="Enter task title"
                    required
                />
            </div>

            {/* Description */}
            <div>
                <label htmlFor="description" className="label">
                    Description
                </label>
                <textarea
                    id="description"
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    className="input min-h-[100px] resize-y"
                    placeholder="Add a description..."
                    rows={4}
                />
            </div>

            {/* Priority & Status */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                    <label htmlFor="priority" className="label">
                        Priority
                    </label>
                    <select
                        id="priority"
                        value={formData.priority}
                        onChange={(e) => setFormData({ ...formData, priority: e.target.value as TaskPriority })}
                        className="input"
                    >
                        <option value="Low">Low</option>
                        <option value="Medium">Medium</option>
                        <option value="High">High</option>
                        <option value="Urgent">Urgent</option>
                    </select>
                </div>

                {task && (
                    <div>
                        <label htmlFor="status" className="label">
                            Status
                        </label>
                        <select
                            id="status"
                            value={formData.status}
                            onChange={(e) => setFormData({ ...formData, status: e.target.value as TaskStatus })}
                            className="input"
                        >
                            <option value="Todo">To Do</option>
                            <option value="InProgress">In Progress</option>
                            <option value="InReview">In Review</option>
                            <option value="Done">Done</option>
                        </select>
                    </div>
                )}
            </div>

            {/* Due Date */}
            <div>
                <label htmlFor="dueDate" className="label">
                    Due Date
                </label>
                <input
                    type="date"
                    id="dueDate"
                    value={formData.dueDate}
                    onChange={(e) => setFormData({ ...formData, dueDate: e.target.value })}
                    className="input"
                />
            </div>

            {/* Actions */}
            <div className="flex justify-end gap-3 pt-4">
                <button
                    type="button"
                    onClick={() => router.back()}
                    className="btn-secondary"
                    disabled={loading}
                >
                    Cancel
                </button>
                <button type="submit" className="btn-primary" disabled={loading}>
                    {loading ? (
                        <span className="flex items-center gap-2">
                            <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white" />
                            Saving...
                        </span>
                    ) : task ? (
                        'Update Task'
                    ) : (
                        'Create Task'
                    )}
                </button>
            </div>
        </form>
    );
}

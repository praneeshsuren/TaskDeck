'use client';

import { useState, useEffect } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import { api } from '@/lib/apiClient';
import type { Task, TaskPriority, TaskStatus, Project } from '@/types/todo';

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
    projectId: string;
}

export function TaskForm({ task, onSuccess }: TaskFormProps) {
    const router = useRouter();
    const searchParams = useSearchParams();
    const [loading, setLoading] = useState(false);
    const [projects, setProjects] = useState<Project[]>([]);
    const [error, setError] = useState<string | null>(null);

    const [formData, setFormData] = useState<FormData>({
        title: task?.title || '',
        description: task?.description || '',
        priority: task?.priority || 'Medium',
        status: task?.status || 'Todo',
        dueDate: task?.dueDate ? new Date(task.dueDate).toISOString().split('T')[0] : '',
        projectId: task?.projectId || searchParams.get('projectId') || '',
    });

    useEffect(() => {
        const fetchProjects = async () => {
            try {
                const response = await api.getProjects();
                setProjects(response.data);
                // If no project selected and we have projects, select the first one
                if (!formData.projectId && response.data.length > 0) {
                    setFormData(prev => ({ ...prev, projectId: response.data[0].id }));
                }
            } catch (err) {
                console.error('Failed to fetch projects:', err);
            }
        };
        fetchProjects();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (!formData.projectId) {
            setError('Please select a project');
            return;
        }

        setLoading(true);

        // Map string priority to numeric values for backend
        const priorityMap: Record<string, number> = {
            'Low': 0,
            'Medium': 1,
            'High': 2,
            'Urgent': 3
        };

        const statusMap: Record<string, number> = {
            'Todo': 0,
            'InProgress': 1,
            'InReview': 2,
            'Done': 3,
            'Cancelled': 4
        };

        try {
            const payload = {
                title: formData.title,
                description: formData.description || undefined,
                priority: priorityMap[formData.priority] ?? 1,
                dueDate: formData.dueDate ? new Date(formData.dueDate).toISOString() : undefined,
            };

            if (task) {
                // Update existing task
                await api.updateTask(task.id, { ...payload, status: statusMap[formData.status] ?? 0 });
            } else {
                // Create new task
                await api.createTask(formData.projectId, payload);
            }

            if (onSuccess) {
                onSuccess();
            } else {
                router.push(`/projects/${formData.projectId}`);
            }
        } catch (err) {
            console.error('Error saving task:', err);
            setError('Failed to save task. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-6">
            {/* Error Message */}
            {error && (
                <div className="p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg text-red-600 dark:text-red-400 text-sm">
                    {error}
                </div>
            )}

            {/* Project Selector */}
            <div>
                <label htmlFor="projectId" className="label">
                    Project <span className="text-red-500">*</span>
                </label>
                <select
                    id="projectId"
                    value={formData.projectId}
                    onChange={(e) => setFormData({ ...formData, projectId: e.target.value })}
                    className="input"
                    required
                >
                    <option value="">Select a project</option>
                    {projects.map((project) => (
                        <option key={project.id} value={project.id}>
                            {project.name}
                        </option>
                    ))}
                </select>
                {projects.length === 0 && (
                    <p className="text-sm text-gray-500 mt-1">
                        No projects yet. Create a project first.
                    </p>
                )}
            </div>

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

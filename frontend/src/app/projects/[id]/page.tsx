'use client';

import { useState, useEffect, useCallback } from 'react';
import { useParams } from 'next/navigation';
import Link from 'next/link';
import { TaskCard } from '@/components/TaskCard';
import { ProjectSidebar } from '@/components/ProjectSidebar';
import { InviteModal } from '@/components/InviteModal';
import { api } from '@/lib/apiClient';
import { Plus, Search, Filter, ArrowLeft, UserPlus } from 'lucide-react';
import type { Task, Project } from '@/types/todo';

// Helper to map numeric enum values to strings
const statusMap: Record<number, string> = {
    0: 'Todo',
    1: 'InProgress',
    2: 'InReview',
    3: 'Done',
    4: 'Cancelled'
};
const priorityMap: Record<number, string> = {
    0: 'Low',
    1: 'Medium',
    2: 'High',
    3: 'Urgent'
};

function transformTask(task: any): Task {
    return {
        ...task,
        status: typeof task.status === 'number' ? statusMap[task.status] : task.status,
        priority: typeof task.priority === 'number' ? priorityMap[task.priority] : task.priority,
    };
}

export default function ProjectTasksPage() {
    const params = useParams();
    const projectId = params.id as string;

    const [project, setProject] = useState<Project | null>(null);
    const [projects, setProjects] = useState<Project[]>([]);
    const [tasks, setTasks] = useState<Task[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState('');
    const [statusFilter, setStatusFilter] = useState<string>('all');
    const [inviteModalOpen, setInviteModalOpen] = useState(false);

    const fetchProjects = useCallback(async () => {
        try {
            const response = await api.getProjects();
            setProjects(response.data);
        } catch (error) {
            console.error('Failed to fetch projects:', error);
        }
    }, []);

    const fetchProjectData = useCallback(async () => {
        if (!projectId) return;

        setLoading(true);
        try {
            const [projectRes, tasksRes] = await Promise.all([
                api.getProject(projectId),
                api.getTasks(projectId)
            ]);
            setProject(projectRes.data);
            
            // Transform tasks to use string status/priority
            const transformedTasks = tasksRes.data.map(transformTask);
            setTasks(transformedTasks);
        } catch (error) {
            console.error('Failed to fetch project data:', error);
        } finally {
            setLoading(false);
        }
    }, [projectId]);

    useEffect(() => {
        fetchProjects();
        fetchProjectData();
    }, [fetchProjects, fetchProjectData]);

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
        <div className="flex h-[calc(100vh-64px)]">
            <ProjectSidebar projects={projects} onProjectCreated={fetchProjects} />

            <div className="flex-1 overflow-auto p-6">
                <div className="max-w-6xl mx-auto">
                    {/* Header */}
                    <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 mb-8">
                        <div className="flex items-center gap-4">
                            <Link 
                                href="/dashboard" 
                                className="p-2 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 rounded-lg hover:bg-gray-100 dark:hover:bg-dark-border"
                            >
                                <ArrowLeft className="w-5 h-5" />
                            </Link>
                            <div>
                                <div className="flex items-center gap-3">
                                    {project?.color && (
                                        <div 
                                            className="w-4 h-4 rounded-full" 
                                            style={{ backgroundColor: project.color }}
                                        />
                                    )}
                                    <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                                        {project?.name || 'Loading...'}
                                    </h1>
                                </div>
                                {project?.description && (
                                    <p className="text-gray-600 dark:text-gray-400 mt-1">
                                        {project.description}
                                    </p>
                                )}
                            </div>
                        </div>
                        <div className="flex items-center gap-2">
                            <button
                                onClick={() => setInviteModalOpen(true)}
                                className="btn-secondary inline-flex items-center gap-2"
                            >
                                <UserPlus className="w-5 h-5" />
                                Invite
                            </button>
                            <Link 
                                href={`/tasks/new?projectId=${projectId}`} 
                                className="btn-primary inline-flex items-center gap-2"
                            >
                                <Plus className="w-5 h-5" />
                                New Task
                            </Link>
                        </div>
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
                                            statusTasks.map((task) => (
                                                <TaskCard key={task.id} task={task} />
                                            ))
                                        )}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}

                    {/* Empty state */}
                    {!loading && tasks.length === 0 && (
                        <div className="text-center py-12">
                            <p className="text-gray-500 dark:text-gray-400 mb-4">
                                No tasks in this project yet
                            </p>
                            <Link 
                                href={`/tasks/new?projectId=${projectId}`}
                                className="btn-primary inline-flex items-center gap-2"
                            >
                                <Plus className="w-5 h-5" />
                                Create your first task
                            </Link>
                        </div>
                    )}
                </div>
            </div>

            {/* Invite Modal */}
            {project && (
                <InviteModal
                    projectId={projectId}
                    projectName={project.name}
                    isOpen={inviteModalOpen}
                    onClose={() => setInviteModalOpen(false)}
                />
            )}
        </div>
    );
}

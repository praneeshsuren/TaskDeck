export type TaskStatus = 'Todo' | 'InProgress' | 'InReview' | 'Done' | 'Cancelled';
export type TaskPriority = 'Low' | 'Medium' | 'High' | 'Urgent';

export interface User {
    id: string;
    email: string;
    displayName: string;
    avatarUrl?: string;
}

export interface Task {
    id: string;
    title: string;
    description?: string;
    status: TaskStatus;
    priority: TaskPriority;
    order: number;
    dueDate?: string;
    completedAt?: string;
    createdAt: string;
    updatedAt?: string;
    projectId: string;
    assignedTo?: User;
    createdBy: User;
}

export interface Project {
    id: string;
    name: string;
    description?: string;
    color?: string;
    icon?: string;
    isArchived: boolean;
    createdAt: string;
    updatedAt?: string;
    isOwner?: boolean;
}

export interface CreateTaskDto {
    title: string;
    description?: string;
    priority: TaskPriority;
    dueDate?: string;
    assignedToId?: string;
}

export interface UpdateTaskDto {
    title?: string;
    description?: string;
    status?: TaskStatus;
    priority?: TaskPriority;
    order?: number;
    dueDate?: string;
    assignedToId?: string;
}

export interface CreateProjectDto {
    name: string;
    description?: string;
    color?: string;
    icon?: string;
}

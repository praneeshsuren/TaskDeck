export interface ApiResponse<T> {
    data: T;
    message?: string;
}

export interface ApiError {
    code: string;
    message: string;
    traceId: string;
    errors?: Record<string, string[]>;
}

export interface PaginatedResponse<T> {
    items: T[];
    total: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface AuthResponse {
    token: string;
    expiresAt: string;
    user: {
        id: string;
        email: string;
        displayName: string;
        avatarUrl?: string;
    };
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
    owner: User;
    taskCount: number;
    isOwner: boolean;
}

export interface User {
    id: string;
    email: string;
    displayName: string;
    avatarUrl?: string;
}

export interface Invitation {
    id: string;
    projectId: string;
    projectName: string;
    projectColor?: string;
    invitedUser: User;
    invitedBy: User;
    status: number;
    createdAt: string;
}

export interface ProjectMember {
    id: string;
    user: User;
    role: number;
    joinedAt: string;
    isOwner: boolean;
}

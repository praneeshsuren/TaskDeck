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

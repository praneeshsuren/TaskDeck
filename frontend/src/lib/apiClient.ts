import axios from 'axios';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

export const apiClient = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor to add auth token
apiClient.interceptors.request.use(
    (config) => {
        // Use the JWT token from localStorage (set after backend auth)
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // Clear invalid tokens
            localStorage.removeItem('token');
            localStorage.removeItem('tokenExpiry');
            // Don't redirect - just reject the error and let the component handle it
            console.error('Unauthorized - token may be expired');
        }
        return Promise.reject(error);
    }
);

// API helper functions
export const api = {
    // Tasks
    getTasks: (projectId: string) => apiClient.get(`/api/projects/${projectId}/tasks`),
    getTask: (taskId: string) => apiClient.get(`/api/tasks/${taskId}`),
    createTask: (projectId: string, data: unknown) =>
        apiClient.post(`/api/projects/${projectId}/tasks`, data),
    updateTask: (taskId: string, data: unknown) =>
        apiClient.put(`/api/tasks/${taskId}`, data),
    deleteTask: (taskId: string) => apiClient.delete(`/api/tasks/${taskId}`),

    // Projects
    getProjects: () => apiClient.get('/api/projects'),
    getProject: (projectId: string) => apiClient.get(`/api/projects/${projectId}`),
    createProject: (data: unknown) => apiClient.post('/api/projects', data),
    updateProject: (projectId: string, data: unknown) =>
        apiClient.put(`/api/projects/${projectId}`, data),
    deleteProject: (projectId: string) => apiClient.delete(`/api/projects/${projectId}`),

    // Users
    getCurrentUser: () => apiClient.get('/api/users/me'),
    updateProfile: (data: unknown) => apiClient.put('/api/users/me', data),
};

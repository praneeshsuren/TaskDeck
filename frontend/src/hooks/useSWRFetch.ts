'use client';

import useSWR, { SWRConfiguration } from 'swr';
import { apiClient } from '@/lib/apiClient';

const fetcher = async (url: string) => {
    const response = await apiClient.get(url);
    return response.data;
};

export function useSWRFetch<T>(
    url: string | null,
    options?: SWRConfiguration
) {
    const { data, error, isLoading, mutate } = useSWR<T>(
        url,
        fetcher,
        {
            revalidateOnFocus: false,
            ...options,
        }
    );

    return {
        data,
        error,
        isLoading,
        mutate,
    };
}

export function useTasks(projectId?: string) {
    const url = projectId ? `/api/projects/${projectId}/tasks` : null;
    return useSWRFetch(url);
}

export function useTask(taskId?: string) {
    const url = taskId ? `/api/tasks/${taskId}` : null;
    return useSWRFetch(url);
}

export function useProjects() {
    return useSWRFetch('/api/projects');
}

export function useProject(projectId?: string) {
    const url = projectId ? `/api/projects/${projectId}` : null;
    return useSWRFetch(url);
}

'use client';

import { useEffect, useState, useCallback } from 'react';
import { HubConnection } from '@microsoft/signalr';
import { createSignalRConnection } from '@/lib/signalRClient';

interface UseSignalROptions {
    projectId?: string;
    onTaskCreated?: (task: unknown) => void;
    onTaskUpdated?: (task: unknown) => void;
    onTaskDeleted?: (taskId: string) => void;
}

export function useSignalR({
    projectId,
    onTaskCreated,
    onTaskUpdated,
    onTaskDeleted,
}: UseSignalROptions) {
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const hubConnection = createSignalRConnection();

        hubConnection.on('TaskCreated', (task) => {
            onTaskCreated?.(task);
        });

        hubConnection.on('TaskUpdated', (task) => {
            onTaskUpdated?.(task);
        });

        hubConnection.on('TaskDeleted', (taskId) => {
            onTaskDeleted?.(taskId);
        });

        hubConnection
            .start()
            .then(() => {
                setIsConnected(true);
                setConnection(hubConnection);

                if (projectId) {
                    hubConnection.invoke('JoinProject', projectId);
                }
            })
            .catch((err) => console.error('SignalR Connection Error:', err));

        return () => {
            if (hubConnection) {
                if (projectId) {
                    hubConnection.invoke('LeaveProject', projectId);
                }
                hubConnection.stop();
            }
        };
    }, [projectId, onTaskCreated, onTaskUpdated, onTaskDeleted]);

    const joinProject = useCallback(
        async (id: string) => {
            if (connection && isConnected) {
                await connection.invoke('JoinProject', id);
            }
        },
        [connection, isConnected]
    );

    const leaveProject = useCallback(
        async (id: string) => {
            if (connection && isConnected) {
                await connection.invoke('LeaveProject', id);
            }
        },
        [connection, isConnected]
    );

    return {
        isConnected,
        joinProject,
        leaveProject,
    };
}

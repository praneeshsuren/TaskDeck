import { HubConnectionBuilder, HubConnection, LogLevel } from '@microsoft/signalr';
import { auth } from './firebaseClient';

const SIGNALR_URL = process.env.NEXT_PUBLIC_SIGNALR_URL || 'http://localhost:5000/hubs/tasks';

export function createSignalRConnection(): HubConnection {
    return new HubConnectionBuilder()
        .withUrl(SIGNALR_URL, {
            accessTokenFactory: async () => {
                const user = auth.currentUser;
                if (user) {
                    return await user.getIdToken();
                }
                return '';
            },
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
        .configureLogging(LogLevel.Information)
        .build();
}

export type SignalRConnection = HubConnection;

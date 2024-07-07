import { Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

@Injectable({
    providedIn: 'root'
})
export class WebSocketService {
    private readonly connection: HubConnection = new HubConnectionBuilder()
        .withUrl('https://localhost:5000/wss/client')
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();

    public isConnected$ = signal<boolean>(false);
    public hasDisconnected$ = signal<boolean>(false);
    public static connectionId = '';

    public init() {
        this.connection.start()
            .then(() => {
                this.onConnect();
            })
            .catch(err => {
                console.log('Failed to connect');
                console.error(err);
            })

        this.connection.onclose(err => {
            console.log('Disconnected. Reason: ' + err);
            this.isConnected$.set(false);
            this.hasDisconnected$.set(true);
        })

        this.connection.onreconnected(() => {
            this.onConnect();
        })
    }

    private onConnect() {
        this.connection.invoke('Connect')
            .then(connectionId => {
                WebSocketService.connectionId = connectionId;
                this.isConnected$.set(true);
            })
            .catch(err => {
                console.error(err);
            })
    }
}

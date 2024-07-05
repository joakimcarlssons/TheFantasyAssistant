import { Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Player } from '../models/player.model';

@Injectable({
    providedIn: 'root'
})
export class WebSocketService {
    private readonly connection: HubConnection = new HubConnectionBuilder()
        .withUrl('https://localhost:5000/wss/client')
        .configureLogging(LogLevel.Information)
        .build();

    public data$ = signal<Player[]>([]);

    public init() {
        this.connection.start()
            .then(() => {
                console.log('Connection established.');

                this.setupHandlers();
                this.loadData();
            })
            .catch(err => {
                console.log('Failed to connect');
                console.error(err);
            })

        this.connection.onclose(err => {
            console.log('Disconnected. Reason: ' + err);
        })   
    }

    private setupHandlers() {
        this.connection.on('GetFantasyData', data => {
            this.data$.set(data.players);
        })
    }

    private loadData() {
        this.connection.invoke('GetFantasyData')
            .catch(err => console.error(err));
    }
}
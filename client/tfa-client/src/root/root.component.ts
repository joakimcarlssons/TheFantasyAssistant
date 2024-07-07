import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BaseDataService } from '../data/services/baseData.service';
import { CommonModule } from '@angular/common';
import { WebSocketService } from '../data/services/websocket.service';

@Component({
    selector: 'tfa-root',
    standalone: true,
    imports: [
        CommonModule,
        RouterOutlet,
        MatToolbarModule
    ],
    templateUrl: './root.component.html',
})

export class AppComponent {
    title = 'tfa-client';

    public constructor(
        public readonly wss: WebSocketService,
        public readonly baseData: BaseDataService
    ) {}
}

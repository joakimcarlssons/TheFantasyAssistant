import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { BaseDataService } from '../data/services/baseData.service';
import { CommonModule } from '@angular/common';
import { WebSocketService } from '../data/services/websocket.service';
import { NgMaterialModule } from '../modules/ngMaterial.module';
import { PageLoaderComponent } from '../components/loaders/pageLoader.component';

@Component({
    selector: 'tfa-root',
    standalone: true,
    imports: [
        CommonModule,
        RouterOutlet,
        RouterModule,
        NgMaterialModule,
        PageLoaderComponent,
    ],
    templateUrl: './root.component.html',
})

export class AppComponent {
    public constructor(
        public readonly wss: WebSocketService,
        public readonly baseData: BaseDataService
    ) {}
}

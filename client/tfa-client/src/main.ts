import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { WebSocketService } from './data/services/websocket.service';

bootstrapApplication(AppComponent, appConfig)
    .then(app => {
        const wss = app.injector.get(WebSocketService);
        wss.init();
    })
    .catch((err) => console.error(err));

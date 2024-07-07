import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './root/root.config';
import { AppComponent } from './root/root.component';
import { WebSocketService } from './data/services/websocket.service';

bootstrapApplication(AppComponent, appConfig)
    .then(app => {
        const wss = app.injector.get(WebSocketService);
        wss.init();
    })
    .catch((err) => console.error(err));

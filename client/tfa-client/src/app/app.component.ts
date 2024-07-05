import { Component, Signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { WebSocketService } from '../data/services/websocket.service';
import { Player } from '../data/models/player.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatToolbarModule],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'tfa-client';

  public data: Signal<Player[]>;

  public constructor(private readonly wss: WebSocketService) {
    wss.init();
    this.data = wss.data$;
  }
}

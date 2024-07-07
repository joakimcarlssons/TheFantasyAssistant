import { Component, Input, OnInit } from "@angular/core";
import { WebSocketService } from "../../data/services/websocket.service";
import { CommonModule } from "@angular/common";

@Component({
    selector: 'tfa-page-loader',
    standalone: true,
    imports: [
        CommonModule
    ],
    template: `
        @if (hasWaited) {
            <div class="w-full h-full flex flex-col justify-center items-center">
                <img src="tfa_logo_compressed.png" alt="logo" class="h-72" [ngClass]="{ 'animate-pulse': !wss.hasDisconnected$() }" />

                <p class="mt-12 font-bold transition-all ease-in duration-300 opacity-0" [ngClass]="{ 'opacity-100': wss.hasDisconnected$() }">
                  Could not connect to server.
                </p>
            </div>
        }
    `,
    styles: [':host { @apply h-screen flex -mt-7 }']
})
export class PageLoaderComponent implements OnInit {

    /** The delay to wait before showing the loader. */
    @Input() public delayInMs = 0;

    /** The flag to indicate if the content should be shown or not. */
    protected hasWaited = false;

    public constructor (public readonly wss: WebSocketService) {}

    public ngOnInit(): void {
        setTimeout(() => {
            this.hasWaited = true;
        }, this.delayInMs);
    }
}

import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { WebSocketService } from "../services/websocket.service";

@Injectable()
export class DefaultHttpHeaderInterceptor implements HttpInterceptor {
    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        const clonedRequest = request.clone({
            setHeaders: {
                'x-client-connection-id': WebSocketService.connectionId
            }
        })

        return next.handle(clonedRequest);
    }
}

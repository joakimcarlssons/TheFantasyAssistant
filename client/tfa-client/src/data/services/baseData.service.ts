import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { shareReplay } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class BaseDataService {
    private readonly baseUrl = 'https://localhost:5000/client';

    public constructor(private readonly http: HttpClient) {}

    public readonly data$ = this.http.get<string>(`${this.baseUrl}/data`)
        .pipe(shareReplay(1))
}

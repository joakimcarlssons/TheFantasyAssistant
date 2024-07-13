import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map, shareReplay } from "rxjs";
import { BaseData } from "../models/baseData.model";
import { environment } from "../../environments/environment";

@Injectable({
    providedIn: 'root'
})
export class BaseDataService {
    private readonly baseUrl = `${environment.API_URL}/client`;

    public constructor(private readonly http: HttpClient) {}

    private readonly data$ = this.http.get<BaseData>(`${this.baseUrl}/data`)
        .pipe(shareReplay(1))

    public readonly players$ = this.data$.pipe(map(data => data.players));
}

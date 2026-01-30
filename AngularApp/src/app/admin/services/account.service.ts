import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountModel } from '../models/accountModel';
import { ApiResponse } from '../models/apiResponse';
import { environment } from '../../../environments/environment';


@Injectable({
    providedIn: 'root'
})
export class AccountService {
    private apiUrl = `${environment.apiUrl}/account`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<AccountModel[]> {
        return this.http.get<ApiResponse<AccountModel[]>>(this.apiUrl)
            .pipe(map(response => response.data));
    }
}

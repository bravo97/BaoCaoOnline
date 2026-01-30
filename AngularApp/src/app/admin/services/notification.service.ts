import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Notification } from '../models/notificationModel';
import { ApiResponse } from '../models/apiResponse';
import { environment } from '../../../environments/environment';


@Injectable({
    providedIn: 'root'
})
export class NotificationService {
    private apiUrl = `${environment.apiUrl}/notification`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<Notification[]> {
        return this.http.get<ApiResponse<Notification[]>>(this.apiUrl)
            .pipe(map(response => response.data));
    }

    create(notification: Notification): Observable<Notification> {
        return this.http.post<Notification>(this.apiUrl, notification);
    }

    update(notification: Notification): Observable<any> {
        return this.http.put(`${this.apiUrl}`, notification);
    }

    delete(id: string): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`);
    }
}

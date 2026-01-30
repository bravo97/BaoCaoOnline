import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface UserNotification {
    id: string;
    title: string;
    message: string;
    isRead: boolean;
    createdAt: string;
    type?: string;
    data?: string; // Example: FeedbackID for linking
}

@Injectable({
    providedIn: 'root'
})
export class UserNotificationService {
    private apiUrl = `${environment.apiUrl}/notification`;

    constructor(private http: HttpClient) { }

    getMyNotifications(page: number = 1, pageSize: number = 20): Observable<any> {
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());
        return this.http.get<any>(`${this.apiUrl}/my-notifications`, { params });
    }

    markAsRead(id: string): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/${id}/read`, {});
    }

    getUnreadCount(): Observable<any> {
        return this.http.get<any>(`${this.apiUrl}/unread-count`);
    }
}

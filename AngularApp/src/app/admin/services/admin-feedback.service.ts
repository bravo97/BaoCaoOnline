import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Feedback } from '../models/feedbackModel';
import { ApiResponse } from '../models/apiResponse';

@Injectable({
    providedIn: 'root'
})
export class AdminFeedbackService {
    private baseUrl = `${environment.apiUrl}/feedback`;

    constructor(private http: HttpClient) { }

    getAll(page: number = 1, pageSize: number = 20): Observable<ApiResponse<Feedback[]>> {
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());
        return this.http.get<ApiResponse<Feedback[]>>(this.baseUrl, { params });
    }

    getById(id: string): Observable<ApiResponse<Feedback>> {
        return this.http.get<ApiResponse<Feedback>>(`${this.baseUrl}/${id}`);
    }

    respond(id: string, response: string): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}/${id}/response`, { response });
    }

    updateStatus(id: string, status: number): Observable<void> {
        const params = new HttpParams().set('status', status.toString());
        return this.http.put<void>(`${this.baseUrl}/${id}/status`, {}, { params });
    }
}

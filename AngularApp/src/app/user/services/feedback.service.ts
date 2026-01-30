import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class FeedbackService {
    private apiUrl = `${environment.apiUrl}/feedback`;

    constructor(private http: HttpClient) { }

    create(feedback: any): Observable<any> {
        return this.http.post<any>(this.apiUrl, feedback);
    }
}

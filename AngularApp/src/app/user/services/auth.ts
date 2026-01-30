import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CustomerVerificationResponse } from '../models/customerVerificationModel';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`; // endpoint gốc
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private http: HttpClient) { }

  // 🔒 Login: tự lưu token bên trong service
  login(username: string, password: string, customerKey?: string) {
    const payload: any = { username, password };
    if (customerKey) {
      payload.customerId = customerKey;
    }
    return this.http.post<any>(`${this.apiUrl}/loginuser`, payload)
      .pipe(
        tap(response => {
          // Lưu token
          sessionStorage.setItem('userAccessToken', response.token);
          if (response.refreshToken) {
            sessionStorage.setItem('userRefreshToken', response.refreshToken);
          }

          // Cập nhật trạng thái đăng nhập
          this.isLoggedInSubject.next(true);
        })
      );
  }

  logout() {
    sessionStorage.removeItem('userAccessToken');
    sessionStorage.removeItem('userRefreshToken');
    this.isLoggedInSubject.next(false);
  }

  getToken() {
    return sessionStorage.getItem('userAccessToken');
  }

  getRefreshToken() {
    return sessionStorage.getItem('userRefreshToken');
  }

  refreshToken(token: string, refreshToken: string) {
    // Backend expects [FromBody] string -> JSON string of the token
    return this.http.post<any>(`${this.apiUrl}/refresh`, JSON.stringify(refreshToken), {
      headers: { 'Content-Type': 'application/json' }
    })
      .pipe(
        tap(response => {
          sessionStorage.setItem('userAccessToken', response.token || response.accessToken);
          if (response.refreshToken) {
            sessionStorage.setItem('userRefreshToken', response.refreshToken);
          }
        })
      );
  }

  // 🔑 Verify Customer Key
  verifyCustomerKey(customerKey: string): Observable<CustomerVerificationResponse> {
    return this.http.get<CustomerVerificationResponse>(`${this.apiUrl}/verification?customerId=${customerKey}`);
  }

  hasToken(): boolean {
    return !!sessionStorage.getItem('userAccessToken');
  }
}

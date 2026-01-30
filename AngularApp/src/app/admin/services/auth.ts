import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`; // endpoint gốc
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private http: HttpClient) { }

  // 🔒 Login: tự lưu token bên trong service
  login(username: string, password: string) {
    console.log(username, password);

    return this.http.post<any>(`${this.apiUrl}/login`, { username, password })
      .pipe(
        tap(response => {
          // Lưu token
          sessionStorage.setItem('adminAccessToken', response.accessToken);
          if (response.refreshToken) {
            sessionStorage.setItem('adminRefreshToken', response.refreshToken);
          }

          // Cập nhật trạng thái đăng nhập
          this.isLoggedInSubject.next(true);
        })
      );
  }

  logout() {
    sessionStorage.removeItem('adminAccessToken');
    sessionStorage.removeItem('adminRefreshToken');
    this.isLoggedInSubject.next(false);
  }

  getToken() {
    return sessionStorage.getItem('adminAccessToken');
  }

  getRefreshToken() {
    return sessionStorage.getItem('adminRefreshToken');
  }

  refreshToken(token: string, refreshToken: string) {
    return this.http.post<any>(`${this.apiUrl}/refresh`, JSON.stringify(refreshToken), {
      headers: { 'Content-Type': 'application/json' }
    })
      .pipe(
        tap(response => {
          sessionStorage.setItem('adminAccessToken', response.accessToken || response.token);
          if (response.refreshToken) {
            sessionStorage.setItem('adminRefreshToken', response.refreshToken);
          }
        })
      );
  }

  hasToken(): boolean {
    return !!sessionStorage.getItem('adminAccessToken');
  }
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:7023/api/auth'; // endpoint gốc
  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private http: HttpClient) {}

  // 🔒 Login: tự lưu token bên trong service
  login(username: string, password: string) {
    return this.http.post<any>(`${this.apiUrl}/loginuser`, { username, password })
      .pipe(
        tap(response => {
          // Lưu token
          localStorage.setItem('accessToken', response.token);

          // Cập nhật trạng thái đăng nhập
          this.isLoggedInSubject.next(true);
        })
      );
  }

  logout() {
    localStorage.removeItem('accessToken');
    this.isLoggedInSubject.next(false);
  }

  getToken() {
    return localStorage.getItem('accessToken');
  }

  hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }
}

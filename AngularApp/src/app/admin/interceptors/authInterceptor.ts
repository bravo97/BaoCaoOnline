import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { AuthService } from '../services/auth';
import { Observable } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  if (req.url.startsWith('/api/admin')) {
    const cloned = req.clone({ setHeaders: { Authorization: 'Bearer admin-token' } });
    return next.handle(cloned);
  }
  return next.handle(req);
 }
}

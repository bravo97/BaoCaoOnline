import { inject, Injectable } from '@angular/core';
import { HttpRequest, HttpInterceptorFn, HttpHandlerFn } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn) => {
  const token = localStorage.getItem('accessToken');
  const router = inject(Router);

  // Thêm Bearer token nếu có
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError(err => {
      if (err.status === 401) {
        // Token hết hạn -> chuyển về login
        localStorage.removeItem('accessToken'); // xoá token cũ
        router.navigate(['login']); // chuyển hướng
      }
      return throwError(() => err);
    })
  );
};
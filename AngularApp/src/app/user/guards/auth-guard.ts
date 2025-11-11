import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.hasToken()) {
    return true; // được phép truy cập
  } else {
    // chuyển hướng về login nếu chưa đăng nhập
    router.navigate(['login']);
    return false;
  }
};

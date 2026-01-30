import { inject } from '@angular/core';
import { HttpRequest, HttpInterceptorFn, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService as AdminAuthService } from '../services/auth';
import { AuthService as UserAuthService } from '../../user/services/auth';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn) => {
  const router = inject(Router);
  const adminAuthService = inject(AdminAuthService);
  const userAuthService = inject(UserAuthService);

  // Xác định context dựa trên URL của trang hiện tại (Client Side)
  // Nếu đang ở trang /admin/... thì là Admin Context
  const currentUrl = router.url;
  const isAdminContext = currentUrl.startsWith('/admin');

  let token = null;

  if (isAdminContext) {
    token = adminAuthService.getToken();
  } else {
    token = userAuthService.getToken();
  }

  // Clone request và thêm token nếu có
  let authReq = req;

  // Logic cũ: luu ý nếu API Login không cần token thì không sao,
  // nhưng nếu API login mà gửi token sai thì coi chừng.
  // Thường API public không check token header hoặc ignore nó.
  if (token) {
    authReq = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      // Bỏ qua verification request lỗi (ko redirect)
      if (req.url.includes('/verification')) {
        return throwError(() => err);
      }

      // Nếu 401 và KHÔNG phải là request login hay refresh-token thì mới handle refresh
      if (err.status === 401 && !req.url.includes('/login') && !req.url.includes('/refresh-token') && !req.url.includes('/refresh')) {

        const refreshToken = isAdminContext ? adminAuthService.getRefreshToken() : userAuthService.getRefreshToken();
        const currentToken = isAdminContext ? adminAuthService.getToken() : userAuthService.getToken();

        if (currentToken && refreshToken) {
          const refresh$ = isAdminContext
            ? adminAuthService.refreshToken(currentToken, refreshToken)
            : userAuthService.refreshToken(currentToken, refreshToken);

          return refresh$.pipe(
            switchMap((res: any) => {
              // Refresh thành công -> Retry request cũ với token mới
              const newToken = res.token; // API trả về token mới
              const newReq = req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } });
              return next(newReq);
            }),
            catchError((refreshErr) => {
              // Refresh thất bại -> Logout
              handleLogout(isAdminContext, adminAuthService, userAuthService, router, currentUrl);
              return throwError(() => refreshErr);
            })
          );
        }
      }

      // Xử lý 401 thông thường hoặc failed refresh
      if (err.status === 401) {
        handleLogout(isAdminContext, adminAuthService, userAuthService, router, currentUrl);
      }

      return throwError(() => err);
    })
  );
};

function handleLogout(
  isAdminContext: boolean,
  adminService: AdminAuthService,
  userService: UserAuthService,
  router: Router,
  currentUrl: string
) {
  if (isAdminContext) {
    adminService.logout();
    if (!currentUrl.includes('/admin/login')) {
      router.navigate(['admin/login']);
    }
  } else {
    userService.logout();
    if (!currentUrl.includes('/login')) {
      router.navigate(['login']);
    }
  }
}
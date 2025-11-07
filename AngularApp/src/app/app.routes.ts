import { Routes } from '@angular/router';
import { LanddingPage } from './landding-page/landding-page';
import { Login } from './user/pages/login/login';

export const routes: Routes = [
  {
    path: '',
    component:LanddingPage
  },
  {
    path: 'login',
    component:Login
  },
  {
    path: '',
    loadChildren: () =>
      import('./user/user.routes').then(m => m.USER_ROUTES),
      pathMatch: 'prefix'  // thay vì mặc định 'full'
  },
  {
    path: 'admin',
    loadChildren: () =>
      import('./admin/admin.routes').then(m => m.ADMIN_ROUTES)
  },
  {
    path: '**',
    redirectTo: '', // nếu route không tồn tại, quay lại landing
  }
];

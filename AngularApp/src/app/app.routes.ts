import { Routes } from '@angular/router';
import { LanddingPage } from './landding-page/landding-page';

export const routes: Routes = [
    {
    path: '',
    component:LanddingPage
  },
  {
    path: '',
    loadChildren: () =>
      import('./user/user.routes').then(m => m.USER_ROUTES)
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

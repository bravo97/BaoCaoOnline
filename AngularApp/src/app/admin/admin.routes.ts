import { Routes } from '@angular/router';
import { Admin } from './admin';
import { Customer } from './pages/customer/customer';
import { Account } from './pages/account/account';
import { Feedback } from './pages/feedback/feedback';
import { Notification } from './pages/notification/notification';
import { Setting } from './pages/setting/setting';
import { Dashboard } from './pages/dashboard/dashboard';
import { authGuard } from './guards/auth-guard';

export const ADMIN_ROUTES: Routes = [
  {
    path:'',
    component:Admin,
    canActivateChild: [authGuard],
    children:[
      { path: '', component: Dashboard },
      { path: 'customers', component: Customer },
      { path: 'accounts', component: Account },
      { path: 'feedbacks', component: Feedback },
      { path: 'notifications', component: Notification },
      { path: 'settings', component: Setting }
    ]
  }  
];
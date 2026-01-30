import { Routes } from '@angular/router';
import { Admin } from './admin';
import { Customer } from './pages/customer/customer';
import { Account } from './pages/account/account';
import { FeedbackComponent } from './pages/feedback/feedback';
import { NotificationList } from './pages/notification/notification';

import { Setting } from './pages/setting/setting';
import { Dashboard } from './pages/dashboard/dashboard';
import { authGuard } from './guards/auth-guard';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    component: Admin,
    canActivateChild: [authGuard],
    children: [
      { path: '', component: Dashboard },
      { path: 'customers', component: Customer },
      { path: 'accounts', component: Account },
      { path: 'feedbacks', component: FeedbackComponent },
      { path: 'notifications', component: NotificationList },

      { path: 'settings', component: Setting }
    ]
  }
];
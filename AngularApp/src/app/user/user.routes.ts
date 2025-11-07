import { Routes } from '@angular/router';
import { User } from './user';
import { ReportList } from './pages/report-list/report-list';
import { ReportViewer } from './pages/report-viewer/report-viewer';
import { Login } from './pages/login/login';

export const USER_ROUTES: Routes = [
  { path: '', component: User },
  { path: 'login', component: Login },
  { path: 'reports/:customer/:code', component: ReportViewer },
  { path: '', redirectTo: 'login', pathMatch: 'full' }  // mặc định khi vào /user
];
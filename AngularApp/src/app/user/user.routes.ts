import { Routes } from '@angular/router';
import { User } from './user';
import { ReportList } from './pages/report-list/report-list';
import { ReportViewer } from './pages/report-viewer/report-viewer';

export const USER_ROUTES: Routes = [
  { path: '', component: User },
  { path: 'reports', component: ReportList },
  { path: 'reports/:customer/:code', component: ReportViewer }
];
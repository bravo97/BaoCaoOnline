import { Routes } from '@angular/router';
import { User } from './user';
import { ReportList } from './pages/report-list/report-list';
import { ReportViewer } from './pages/report-viewer/report-viewer';
import { Login } from './pages/login/login';
import { Home } from './pages/home/home';
import { authGuard } from './guards/auth-guard';

export const USER_ROUTES: Routes = [
  {
      path:'',
      component:User,
      canActivateChild: [authGuard],
      children:[
        { path: 'home', component: Home }
      ]
    }  
];
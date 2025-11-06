import { Routes } from '@angular/router';
import { Admin } from './admin';
import { Customer } from './pages/customer/customer';
import { Account } from './pages/account/account';


export const ADMIN_ROUTES: Routes = [
  { path: '', component: Admin },
  { path: 'customers', component: Customer },
  { path: 'accounts', component: Account },
  // { path: 'customers/edit/:id', component: CustomerForm }
];
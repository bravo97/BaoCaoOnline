import { Routes } from '@angular/router';
import { Admin } from './admin';
import { Customer } from './pages/customer/customer';


export const ADMIN_ROUTES: Routes = [
  { path: '', component: Admin },
  { path: 'customers', component: Customer },
  // { path: 'customers/new', component: CustomerForm },
  // { path: 'customers/edit/:id', component: CustomerForm }
];
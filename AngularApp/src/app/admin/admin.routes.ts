import { Routes } from '@angular/router';
import { Admin } from './admin';
import { CustomerList } from './pages/customer-list/customer-list';
import { CustomerForm } from './pages/customer-form/customer-form';

export const ADMIN_ROUTES: Routes = [
  { path: '', component: Admin },
  { path: 'customers', component: CustomerList },
  { path: 'customers/new', component: CustomerForm },
  { path: 'customers/edit/:id', component: CustomerForm }
];
import { AfterViewInit, Component, ViewChild, ViewEncapsulation } from '@angular/core';
import { CustomerModel } from '../../models/customerModel';
import { CommonModule } from '@angular/common';
import { Header } from "../../layout/header/header";
import { Sidebar } from "../../layout/sidebar/sidebar";
import { CustomerDialog } from '../../dialogs/customer-dialog/customer-dialog';

@Component({
  selector: 'app-customer',
  standalone:true,
  imports: [CommonModule, Header, Sidebar, CustomerDialog],
  templateUrl: './customer.html',
  styleUrls: [
    '../../admin.scss',
    './customer.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Customer implements AfterViewInit{
  @ViewChild('customerDialog') customerDialog!: CustomerDialog;
  headerTitle = 'Khách hàng';
  customers: CustomerModel[] = [
    {
      id: crypto.randomUUID(),
      name: 'Công ty ABC',
      email: 'abc@example.com',
      ipAddress: '192.168.1.10',
      port: 1433,
      serverName: 'SQLSERVER01',
      userName: 'sa',
      password: '123456',
      databaseName: 'CustomerDB',
      note: 'Khách hàng lâu năm'
    },
    {
      id: crypto.randomUUID(),
      name: 'Công ty XYZ',
      email: 'xyz@example.com',
      ipAddress: '10.0.0.5',
      port: 1433,
      serverName: 'SQLSERVER02',
      userName: 'admin',
      password: 'abc123',
      databaseName: 'MainDB',
      note: ''
    }
  ];

  ngAfterViewInit(): void {}
  
  onMenuSelected(menu: string) {
    this.headerTitle = menu;
  }

  openDialog(customer?: CustomerModel | string) {
    if (!this.customerDialog) return;

    if (typeof customer === 'string') {
      // xử lý xóa
      this.customers = this.customers.filter(c => c.id !== customer);
      return;
    }

    this.customerDialog.open(customer);
  }

  saveCustomer(customer: any) {
    const idx = this.customers.findIndex(c => c.id === customer.id);
    if(idx >= 0){
      this.customers[idx] = customer;
    } else {
      customer.id = Date.now().toString();
      this.customers.push(customer);
    }
  }
}

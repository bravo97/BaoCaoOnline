import { AfterViewInit, Component, ViewChild, ViewEncapsulation } from '@angular/core';
import { CustomerModel } from '../../models/customerModel';
import { CommonModule } from '@angular/common';
import { Header } from "../../layout/header/header";
import { Sidebar } from "../../layout/sidebar/sidebar";
import { CustomerDialog } from '../../dialogs/customer-dialog/customer-dialog';
import { CustomerService } from '../../services/customer-service';
import { ToastrNotification } from '../../../shared/services/toastr-service';
import { ConfirmDialog } from "../../../shared/components/confirm-dialog/confirm-dialog";

@Component({
  selector: 'app-customer',
  standalone:true,
  imports: [CommonModule, CustomerDialog, ConfirmDialog],
  templateUrl: './customer.html',
  styleUrl: './customer.scss',
})
export class Customer implements AfterViewInit{
  @ViewChild('customerDialog') customerDialog!: CustomerDialog;
  ustomers = [];
  confirmVisible = false;
  deleteId: string | null = null;
  customers: CustomerModel[] = [];
  constructor(private customerService: CustomerService, private notify:ToastrNotification) {
    this.loadCustomers();
  }
  ngAfterViewInit(): void {}
  
  loadCustomers() {
    this.customerService.getAll().subscribe(data => {
      this.customers = data;
    });
  }

  confirm(id: string) {
    this.deleteId = id;
    this.confirmVisible = true;
  }

  delete() {
    if (this.deleteId) {

      this.customerService.delete(this.deleteId).subscribe(() => {
        this.customers = this.customers.filter(c => c.id !== this.deleteId);
      });
      
      this.deleteId = null;
      this.confirmVisible = false;
    }
  }

  openDialog(customer?: CustomerModel | string) {
    if (!this.customerDialog) return;
    this.customerDialog.open(customer);
  }

  saveCustomer(customer: CustomerModel) {
    if (customer.id) {
      // update
      this.customerService.update(customer).subscribe(updated => {
        this.notify.info(updated.message);
        const idx = this.customers.findIndex(c => c.id === updated.data.id);
        if(idx >= 0) this.customers[idx] = customer;
      });
    } else {
      //tạo mới
      this.customerService.create(customer).subscribe(created => {
        this.notify.success(created.message);
        this.customers.push(created.data);
      });
    }
  }
}

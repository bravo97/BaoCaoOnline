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
  dialogVisible = false;
  dialogTitle = '';
  dialogMessage = '';
  deletingId: string | null = null;
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

  showConfirm(id: string) {
  this.dialogVisible = true;
  this.dialogTitle = 'Xác nhận xóa';
  this.dialogMessage = 'Bạn có chắc muốn xóa khách hàng này không?';
  this.deletingId = id;
}

handleConfirm(result: boolean) {
  if (result && this.deletingId != null) {
    this.customerService.delete(this.deletingId).subscribe({
      next: () => this.notify.success('Xóa khách hàng thành công!'),
      error: () => this.notify.error('Xóa khách hàng thất bại!')
    });
  }
  this.deletingId = null;
}

  openDialog(customer?: CustomerModel | string) {
    if (!this.customerDialog) return;

    if (typeof customer === 'string') {
      // xóa qua API
      this.customerService.delete(customer).subscribe(() => {
        this.customers = this.customers.filter(c => c.id !== customer);
      });
      return;
    }

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

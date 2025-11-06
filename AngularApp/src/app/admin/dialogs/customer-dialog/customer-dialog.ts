import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-customer-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customer-dialog.html',
  styleUrl: './customer-dialog.scss',
})
export class CustomerDialog {
@Output() save = new EventEmitter<any>();
  @Output() close = new EventEmitter<void>();

  visible = false;
  customer: any = {
    id: '',
    name: '',
    email: '',
    ipAddress: '',
    port: 0,
    serverName: '',
    userName: '',
    password: '',
    databaseName: '',
    note: ''
  };

  open(customer?: any) {
    if (customer) {
      this.customer = { ...customer };
    } else {
      this.customer = {
        id: '',
        name: '',
        email: '',
        ipAddress: '',
        port: 0,
        serverName: '',
        userName: '',
        password: '',
        databaseName: '',
        note: ''
      };
    }
    this.visible = true;
  }

  closeDialog() {
    this.visible = false;
    this.close.emit();
  }

  saveCustomer() {
    this.save.emit(this.customer);
    this.closeDialog();
  }
}

import { Component, EventEmitter, Output } from '@angular/core';
import { AccountModel } from '../../models/accountModel';
import { FormsModule } from "@angular/forms";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-account-dialog',
  standalone:true,
  imports: [CommonModule, FormsModule],
  templateUrl: './account-dialog.html',
  styleUrl: './account-dialog.scss',
})
export class AccountDialog {
  @Output() save = new EventEmitter<AccountModel>();
  @Output() close = new EventEmitter<void>();

  visible = false;
  account: AccountModel = {
    id: '',
    customerId: '',
    username: '',
    password: '',
    role: 'Regular',
    note: ''
  };

  open(account?: AccountModel) {
    if (account) {
      this.account = { ...account };
    } else {
      this.account = { id: '', customerId: '', username: '', password: '', role: 'Regular', note: '' };
    }
    this.visible = true;
  }

  closeDialog() {
    this.visible = false;
    this.close.emit();
  }

  saveAccount() {
    this.save.emit(this.account);
    this.closeDialog();
  }
}

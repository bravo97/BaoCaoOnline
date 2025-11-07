import { Component, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountDialog } from '../../dialogs/account-dialog/account-dialog';
import { AccountModel } from '../../models/accountModel';
import { CustomerDialog } from "../../dialogs/customer-dialog/customer-dialog";

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule, AccountDialog],
  templateUrl: './account.html',
  styleUrl:'./account.scss'
})
export class Account implements AfterViewInit {
  @ViewChild('accountDialog') accountDialog!: AccountDialog;

  accounts: AccountModel[] = [
    { id: '1', customerId: 'KH001', username: 'admin', password: '123456', role: 'Admin', note: 'Admin chính' },
    { id: '2', customerId: 'KH001', username: 'user1', password: 'userpass', role: 'Regular', note: 'Nhân viên A' },
    { id: '3', customerId: 'KH002', username: 'user2', password: 'userpass', role: 'Regular', note: 'Nhân viên B' },
    { id: '4', customerId: 'KH003', username: 'user3', password: 'userpass', role: 'Regular', note: '' },
    { id: '5', customerId: 'KH004', username: 'manager', password: 'pass123', role: 'Admin', note: 'Quản lý' }
  ];

  ngAfterViewInit() {}

  openDialog(account?: AccountModel | string) {
      if (!this.accountDialog) return;
  
      if (typeof account === 'string') {
        // xử lý xóa
        this.accounts = this.accounts.filter(c => c.id !== account);
        return;
      }
  
      this.accountDialog.open(account);
    }
  
    saveAccount(account: any) {
      const idx = this.accounts.findIndex(c => c.id === account.id);
      if(idx >= 0){
        this.accounts[idx] = account;
      } else {
        account.id = Date.now().toString();
        this.accounts.push(account);
      }
    }
}

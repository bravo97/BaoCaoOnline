import { Component, AfterViewInit, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountModel } from '../../models/accountModel';
import { AccountService } from '../../services/account.service';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './account.html',
  styleUrl: './account.scss'
})

export class Account implements OnInit {

  accounts: AccountModel[] = [];

  constructor(private accountService: AccountService) { }

  ngOnInit() {
    this.loadAccounts();
  }

  loadAccounts() {
    this.accountService.getAll().subscribe({
      next: (data) => {
        console.log(data);
        this.accounts = data;
      },
      error: (err) => {
        console.error('Failed to load accounts', err);
      }
    });
  }
}

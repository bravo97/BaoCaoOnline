import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { REPORTS } from '../../models/reportModel';

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  @Output() menuSelected = new EventEmitter<string>();
  sidebarCollapsed = false;
  menuItems =  REPORTS;
  
  constructor(private router: Router) {}
  
  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  selectMenu(item: any) {
    this.menuSelected.emit(item.fullName);
  }
}

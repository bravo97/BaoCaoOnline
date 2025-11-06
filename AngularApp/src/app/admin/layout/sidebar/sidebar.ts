import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.html',
  styleUrls: [
    '../../admin.scss',
    './sidebar.scss'],
})
export class Sidebar {
  @Output() menuSelected = new EventEmitter<string>();
  sidebarCollapsed = false;

  constructor(private router: Router) {}

  menuItems = [
    { label: 'Tổng quan', icon: 'fa-solid fa-gauge-high', route:'/admin'},
    { label: 'Khách hàng', icon: 'fa-solid fa-users', route:'/admin/customers' },
    { label: 'Tài khoản', icon: 'fa-solid fa-chart-line', route:'/admin/accounts' },
    { label: 'Thông tin phản ánh', icon: 'fa-solid fa-envelope', route:'/admin/feedback' },
    { label: 'Thiết lập giới hạn', icon: 'fa-solid fa-tasks', route:'/admin/limits' },
    { label: 'Cài đặt hệ thống', icon: 'fa-solid fa-gear', route:'/admin/settings' }
  ];

  // Highlight menu theo URL hiện tại
  get activeMenu() {
    return this.menuItems.find(item => item.route === this.router.url)?.label;
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  selectMenu(item: any) {
    this.menuSelected.emit(item.label);
    if (item.route) {
      this.router.navigate([item.route]);
    }
  }
}

import { Component, ViewEncapsulation } from '@angular/core';
import { Sidebar } from "../../layout/sidebar/sidebar";
import { Header } from "../../layout/header/header";
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification',
  imports: [CommonModule, FormsModule, Sidebar, Header],
  templateUrl: './notification.html',
  styleUrls: [
    '../../admin.scss',
    './notification.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Notification {
  headerTitle = 'Quản lý thông báo';
  notifications = [
    { title: 'Bảo trì hệ thống', message: 'Hệ thống sẽ bảo trì lúc 2AM ngày mai.', date: new Date() },
    { title: 'Khuyến mãi', message: 'Giảm giá 20% cho tất cả sản phẩm.', date: new Date() },
  ];

  title = '';
  message = '';

  onMenuSelected(menu: string) {
    this.headerTitle = menu;
  }

  sendNotification() {
    if (this.title && this.message) {
      this.notifications.unshift({
        title: this.title,
        message: this.message,
        date: new Date()
      });
      this.title = '';
      this.message = '';
    }
  }
}

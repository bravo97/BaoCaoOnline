import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Notification } from '../../models/notificationModel';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './notification.html',
  styleUrl: './notification.scss'
})
export class NotificationList implements OnInit {
  notifications: Notification[] = [];
  selectedNotification: Notification = new Notification();
  isEditMode: boolean = false;
  showModal: boolean = false;

  constructor(private notificationService: NotificationService) { }

  ngOnInit() {
    this.loadNotifications();
  }

  loadNotifications() {
    this.notificationService.getAll().subscribe({
      next: (data) => this.notifications = data,
      error: (err) => console.error('Failed to load notifications', err)
    });
  }

  openModal(notification?: Notification) {
    this.showModal = true;
    if (notification) {
      this.isEditMode = true;
      this.selectedNotification = { ...notification };
    } else {
      this.isEditMode = false;
      this.selectedNotification = new Notification();
    }
  }

  closeModal() {
    this.showModal = false;
  }

  saveNotification() {
    this.selectedNotification.dateUpdate = new Date();

    if (this.isEditMode) {
      this.notificationService.update(this.selectedNotification).subscribe({
        next: () => {
          this.loadNotifications();
          this.closeModal();
        },
        error: (err) => console.error('Failed to update notification', err)
      });
    } else {
      this.selectedNotification.dateCreate = new Date();
      this.notificationService.create(this.selectedNotification).subscribe({
        next: () => {
          this.loadNotifications();
          this.closeModal();
        },
        error: (err) => console.error('Failed to create notification', err)
      });
    }
  }

  deleteNotification(id: string) {
    if (confirm('Bạn có chắc chắn muốn xóa thông báo này?')) {
      this.notificationService.delete(id).subscribe({
        next: () => this.loadNotifications(),
        error: (err) => console.error('Failed to delete notification', err)
      });
    }
  }
}

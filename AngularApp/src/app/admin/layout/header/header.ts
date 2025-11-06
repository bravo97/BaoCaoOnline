import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone:true,
  imports: [CommonModule],
  templateUrl: './header.html',
  styleUrls: [
    '../../admin.scss',
    './header.scss'],
})
export class Header {
  @Input() title: string = 'Tổng quan';
  profileMenuOpen = false;
  isDarkMode = true; // mặc định dark

  toggleDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    const adminContainer = document.querySelector('.admin-container');
    if (this.isDarkMode) {
      adminContainer?.classList.add('dark-mode');
    } else {
      adminContainer?.classList.remove('dark-mode');
    }
  }

  toggleProfileMenu() {
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  logout() {
    console.log('Đăng xuất');
  }

  changePassword() {
    console.log('Đổi mật khẩu');
  }
}

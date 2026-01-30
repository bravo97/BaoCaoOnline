import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.html',
  styleUrls: [
    '../../admin.scss',
    './header.scss'],
})
export class Header implements OnInit {
  @Input() title: string = 'Tổng quan';
  @Output() toggleMenu = new EventEmitter<void>();
  profileMenuOpen = false;
  isDarkMode = true; // mặc định dark

  ngOnInit() {
    const saved = localStorage.getItem('theme') || 'dark';
    this.isDarkMode = saved === 'dark';
    document.documentElement.setAttribute('data-theme', saved);
  }

  toggleDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    document.documentElement.setAttribute('data-theme', this.isDarkMode ? 'dark' : 'light');
    localStorage.setItem('theme', this.isDarkMode ? 'dark' : 'light');
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

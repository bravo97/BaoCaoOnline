import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  @Input() title: string='';
  profileMenuOpen = false;
  isDarkMode = false; // mặc định dark

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

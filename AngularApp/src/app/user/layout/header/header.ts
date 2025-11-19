import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';

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
  constructor(private router: Router){}
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
    localStorage.clear();
    this.router.navigate(['login']);
  }

  changePassword() {
    console.log('Đổi mật khẩu');
  }
}

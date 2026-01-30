import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, ActivatedRoute } from '@angular/router';
import { LayoutService } from '../../../shared/services/layout.service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit {
  title: string = 'Báo Cáo';
  profileMenuOpen = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    public layoutService: LayoutService
  ) { }

  ngOnInit() {
    // Listen to route changes to update title
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updateTitle();
    });

    this.updateTitle();
  }

  updateTitle() {
    // Get title from route data or default
    const reportId = localStorage.getItem('selectedReport');
    if (reportId) {
      try {
        const report = JSON.parse(reportId);
        this.title = report.fullName || 'Báo Cáo';
      } catch {
        this.title = 'Báo Cáo';
      }
    } else {
      this.title = 'Báo Cáo';
    }
  }

  toggleProfileMenu() {
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  changePassword() {
    console.log('Đổi mật khẩu');
  }
}

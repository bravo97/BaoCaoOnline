import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from "./layout/sidebar/sidebar";
import { Header } from "./layout/header/header";

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule, RouterOutlet, Sidebar, Header],
  templateUrl: './user.html',
  styleUrl: './user.scss',
})
export class User implements OnInit {
  sidebarCollapsed = false;
  isReportView = false;

  constructor(private router: Router) {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      // Check if current URL matches report view pattern
      this.isReportView = event.url.includes('/report/');
    });
  }

  ngOnInit(): void {
    // Initial check in case of refresh on report page
    this.isReportView = this.router.url.includes('/report/');
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }
}
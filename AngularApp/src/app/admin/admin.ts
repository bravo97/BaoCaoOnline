import { Component,AfterViewInit, ViewEncapsulation  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Sidebar } from "./layout/sidebar/sidebar";
import { Header } from "./layout/header/header";
import { RouterModule } from "@angular/router";

@Component({
  selector: 'app-admin',
  standalone:true,
  imports: [CommonModule, Sidebar, Header, RouterModule],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
  encapsulation: ViewEncapsulation.None
})
export class Admin {
  sidebarCollapsed = false;
  profileMenuOpen = false;
  headerTitle = 'Tổng quan';

  onMenuSelected(menu: string) {
    this.headerTitle = menu;
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }
}

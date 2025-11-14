import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Sidebar } from "./layout/sidebar/sidebar";
import { Header } from "./layout/header/header";
import { ReportViewer } from "./pages/report-viewer/report-viewer";
import { ReportModel } from './models/reportModel';

@Component({
  selector: 'app-user',
  imports: [CommonModule, Sidebar, Header, ReportViewer],
  templateUrl: './user.html',
  styleUrl: './user.scss',
})
export class User implements OnInit{
  sidebarCollapsed = false;
  profileMenuOpen = false;
  headerTitle = 'Home';
  selectedReport?: ReportModel;

  constructor() {}
  ngOnInit(): void {
    
  }



  selectReport(report: ReportModel) {
    this.headerTitle = report.fullName;
  }

  onMenuSelected(menu: any) {
    
    this.selectedReport = menu;
    this.headerTitle = menu.fullName;
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }
}

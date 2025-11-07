import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Sidebar } from "./layout/sidebar/sidebar";
import { Header } from "./layout/header/header";
import { ReportViewer } from "./pages/report-viewer/report-viewer";
import { ReportModel, REPORTS } from './models/reportModel';

@Component({
  selector: 'app-user',
  imports: [CommonModule, Sidebar, Header, ReportViewer],
  templateUrl: './user.html',
  styleUrl: './user.scss',
})
export class User {
  sidebarCollapsed = false;
  profileMenuOpen = false;
  headerTitle = 'Home';
  reports = REPORTS;           // dữ liệu sidebar
  selectedReport?: ReportModel;

  constructor() {
    // Mặc định chọn report đầu tiên
    this.selectReport(this.reports[0].name);
  }

  selectReport(reportName: string) {
    const report = this.reports.find(r => r.name === reportName);
    if (!report) return;

    this.selectedReport = report;
    this.headerTitle = report.fullName;
  }

  onMenuSelected(menu: string) {
    this.headerTitle = menu;
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }
}

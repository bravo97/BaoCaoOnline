import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReportModel, GroupReportModel } from '../../models/reportModel';
import { Data } from '../../services/data';

@Component({
  selector: 'app-report-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './report-list.html',
  styleUrl: './report-list.scss',
})
export class ReportList implements OnInit {
  reports: ReportModel[] = [];
  groupedReports: Map<string, ReportModel[]> = new Map();
  isLoading = true;

  constructor(
    private router: Router,
    private dataService: Data
  ) { }

  ngOnInit(): void {
    this.loadReports();
  }

  loadReports() {
    this.isLoading = true;
    this.dataService.GetMenuSidebar().subscribe({
      next: (res) => {
        this.reports = res;
        this.groupReportsByCategory();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading reports:', err);
        this.isLoading = false;
      }
    });
  }

  groupReportsByCategory() {
    this.groupedReports.clear();
    this.reports.forEach(report => {
      const group = report.group || 'Khác';
      if (!this.groupedReports.has(group)) {
        this.groupedReports.set(group, []);
      }
      this.groupedReports.get(group)!.push(report);
    });
  }

  getGroups(): string[] {
    return Array.from(this.groupedReports.keys());
  }

  getReportsByGroup(group: string): ReportModel[] {
    return this.groupedReports.get(group) || [];
  }

  navigateToReport(report: ReportModel) {
    this.router.navigate(['/user/report', report.id]);
  }

  getReportIcon(reportName: string): string {
    // Simple icon mapping based on report name
    if (reportName.toLowerCase().includes('doanh thu') || reportName.toLowerCase().includes('revenue')) {
      return 'fa-chart-line';
    } else if (reportName.toLowerCase().includes('khách hàng') || reportName.toLowerCase().includes('customer')) {
      return 'fa-users';
    } else if (reportName.toLowerCase().includes('sản phẩm') || reportName.toLowerCase().includes('product')) {
      return 'fa-box';
    } else if (reportName.toLowerCase().includes('đơn hàng') || reportName.toLowerCase().includes('order')) {
      return 'fa-shopping-cart';
    } else {
      return 'fa-file-alt';
    }
  }
}

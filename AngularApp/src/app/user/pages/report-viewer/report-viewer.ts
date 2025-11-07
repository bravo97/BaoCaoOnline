import { Component, Input, OnChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ColumReportModel, REPORT_COLUMNS, REPORT_DATA, ReportModel, REPORTS } from '../../models/reportModel';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-report-viewer',
  imports: [CommonModule],
  templateUrl: './report-viewer.html',
  styleUrl: './report-viewer.scss',
})
export class ReportViewer implements OnChanges {
  @Input() report?: ReportModel;

  columns: ColumReportModel[] = [];
  data: any[] = [];

  ngOnChanges() {
    if (!this.report) {
      this.columns = [];
      this.data = [];
      return;
    }

    // Lấy cột tương ứng report
    switch(this.report.name) {
      case 'sales-summary':
      case 'monthly-sales':
        this.columns = REPORT_COLUMNS.filter(c => ['date','total','profit'].includes(c.cloumnName));
        break;
      case 'customer-list':
        this.columns = REPORT_COLUMNS.filter(c => ['customerName','email','phone'].includes(c.cloumnName));
        break;
      default:
        this.columns = [];
    }

    // Lấy dữ liệu bảng
    this.data = REPORT_DATA[this.report.name] || [];
  }
}

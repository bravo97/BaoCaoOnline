import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ColumReportModel, ReportModel } from '../../models/reportModel';
import { CommonModule } from '@angular/common';
import { Data } from '../../services/data';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-report-viewer',
  standalone:true,
  imports: [CommonModule,FormsModule],
  templateUrl: './report-viewer.html',
  styleUrl: './report-viewer.scss',
})
export class ReportViewer implements OnChanges {
  @Input() report?: ReportModel;
  columns: ColumReportModel[] = [];
  data: any[] = [];
  fromDate: string | null = null;
  toDate: string | null = null;

  constructor(private service:Data){}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['report']) {
      if(this.report){
        this.loadReport(this.report);
      } 
    }
  }

  loadReport(report: any) {    
    this.service.GetReportDataColumn(report.id).subscribe(
      {
        next: (res) => {
         console.log(res);
         this.columns = res
        //this.isLoading = false;
        },
        error: (err) => {
          console.log(err);
          
          //this.error = 'Không thể tải dữ liệu!';
          //this.isLoading = false;
        }
      }
    )
  }

  applyFilter() {
  console.log('Từ ngày:', this.fromDate);
  console.log('Đến ngày:', this.toDate);

  // Sau này sẽ gọi API theo bộ lọc thật
  // this.reportService.getReport(this.fromDate, this.toDate).subscribe(...)
}
}

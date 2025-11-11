import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ColumReportModel, REPORT_COLUMNS, REPORT_DATA, ReportModel, REPORTS } from '../../models/reportModel';
import { CommonModule } from '@angular/common';
import { Data } from '../../services/data';

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
}

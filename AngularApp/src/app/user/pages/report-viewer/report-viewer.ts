import { ChangeDetectorRef, Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
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
export class ReportViewer implements OnInit,OnChanges {
  @Input() report?: ReportModel;
  columns: ColumReportModel[] = [];
  reportID:string='';
  data: any[] = [];
  fromDate: Date | null = new Date();
  toDate: Date | null = new Date();
  pageIndex = 0;
  pageSize = 50;
  pagedData: any[] = [];
  totalPages = 0;


  constructor(private service:Data,private cd: ChangeDetectorRef){}
  ngOnInit(): void {
    this.fromDate = new Date();
    this.toDate = new Date();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['report']) {
      if(this.report){
        this.columns = [];
        this.data = [];
        this.pagedData = [];
        this.pageIndex = 0;
        this.totalPages = 0;
        
        this.loadReport(this.report);
      } 
    }
  }

  loadReport(report: any) {     
    this.service.GetReportDataColumn(report.id).subscribe(
      {
        next: (res) => {         
         this.reportID = report.id;
         this.columns = res;
        },
        error: (err) => {
          console.log(err);
        }
      }
    )
  }

  applyFilter() {
    this.service.GetReportData(this.reportID).subscribe(
      {
        next: (res) => {
          console.log(res);
          this.data = res
          this.totalPages = Math.ceil(this.data.length / this.pageSize);
          this.refreshPagedData();
        },
        error: (err) => {
          console.log(err);
        }
      }
    );
  }

  refreshPagedData() {
    const start = this.pageIndex * this.pageSize;
    const end = start + this.pageSize;
    this.pagedData = this.data.slice(start, end);
    }

  nextPage() {
    if (this.pageIndex < this.totalPages - 1) {
      this.pageIndex++;
      this.refreshPagedData();
    }
  }

  prevPage() {
    if (this.pageIndex > 0) {
      this.pageIndex--;
      this.refreshPagedData();
    }
  }

}

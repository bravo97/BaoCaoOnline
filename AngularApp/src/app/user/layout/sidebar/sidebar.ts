import { CommonModule } from '@angular/common';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { GroupReportModel, ReportModel } from '../../models/reportModel';
import { Data } from '../../services/data';

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar implements OnInit{
  @Output() menuSelected = new EventEmitter<string>();
  sidebarCollapsed = false;
  groupMenuItems:GroupReportModel[] | undefined;
  menuItems:ReportModel[] | undefined;
  selectedReportId: string | null = null;
  
  constructor(private router: Router,private data: Data) {}
  ngOnInit(): void {
    this.data.GetMenuSidebar().subscribe(res=>{
      this.menuItems = res;
    });

    const reported = localStorage.getItem("selectedReport");
    if (reported) {
      this.selectedReportId = JSON.parse(reported).id;  
      // 👉 emit lại để cha nhận report và load data
      this.menuSelected.emit(JSON.parse(reported));
    }
  }
  
  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  selectMenu(item: any) {
    this.menuSelected.emit(item);
    this.selectedReportId = item.id
    localStorage.setItem("selectedReport", JSON.stringify(item));
  }
}

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
  
  constructor(private router: Router,private data: Data) {}
  ngOnInit(): void {
    this.data.GetMenuSidebar().subscribe(res=>{
      this.menuItems = res;
    });
  }
  
  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  selectMenu(item: any) {
    this.menuSelected.emit(item);
  }
}

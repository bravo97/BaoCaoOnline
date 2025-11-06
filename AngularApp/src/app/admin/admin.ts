import { Component,AfterViewInit, ViewEncapsulation  } from '@angular/core';
import { CommonModule } from '@angular/common';
import Chart from 'chart.js/auto';
import { Sidebar } from "./layout/sidebar/sidebar";
import { Header } from "./layout/header/header";

@Component({
  selector: 'app-admin',
  standalone:true,
  imports: [CommonModule, Sidebar, Header],
  templateUrl: './admin.html',
  styleUrl: './admin.scss',
  encapsulation: ViewEncapsulation.None
})
export class Admin {
  sidebarCollapsed = false;
  profileMenuOpen = false;
  orders = [
      { id: 'OR9842', customer: 'Robert Fox', status: 'Completed' },
      { id: 'OR1849', customer: 'Arlene McCoy', status: 'Pending' },
      { id: 'OR7429', customer: 'Glenna Reichert', status: 'Processing' },
      { id: 'OR7429', customer: 'Clementine Bauch', status: 'Completed' }
    ];
  
  ngAfterViewInit(): void {
    this.initRevenueChart();
    this.initStatsChart();
    this.initTrafficChart();
  }

  initRevenueChart() {
    new Chart('revenueChart', {
      type: 'line',
      data: {
        labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
        datasets: [{
          data: [150, 230, 180, 320, 280, 400, 700],
          borderColor: '#3b82f6',
          fill: true,
          backgroundColor: 'rgba(59,130,246,0.1)',
          tension: 0.4,
        }]
      },
      options: {
        scales: {
          x: { ticks: { color: '#94a3b8' } },
          y: { ticks: { color: '#94a3b8' } }
        },
        plugins: { legend: { display: false } },
      }
    });
  }

  initStatsChart() {
    new Chart('statsChart', {
      type: 'bar',
      data: {
        labels: ['A', 'B', 'C', 'D', 'E', 'F'],
        datasets: [{
          data: [80, 120, 200, 160, 140, 220],
          backgroundColor: '#3b82f6',
          borderRadius: 6
        }]
      },
      options: {
        scales: {
          x: { ticks: { color: '#94a3b8' } },
          y: { ticks: { color: '#94a3b8' } }
        },
        plugins: { legend: { display: false } }
      }
    });
  }

  initTrafficChart() {
    new Chart('trafficChart', {
      type: 'doughnut',
      data: {
        labels: ['Desktop', 'Mobile'],
        datasets: [{
          data: [65, 35],
          backgroundColor: ['#3b82f6', '#1e40af'],
          borderWidth: 0
        }]
      },
      options: {
        plugins: { legend: { labels: { color: '#94a3b8' } } }
      }
    });
  }

  headerTitle = 'Tổng quan';

onMenuSelected(menu: string) {
  this.headerTitle = menu;
}

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  toggleProfileMenu() {
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  changePassword(){

  }
  logout(){

  }
}

import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ReportModel } from '../../models/reportModel';
import { Data } from '../../services/data';
import { ThemeService } from '../../../shared/services/theme.service';
import { AuthService } from '../../services/auth';
import { LayoutService } from '../../../shared/services/layout.service';
import { FeedbackDialog } from '../../dialogs/feedback-dialog/feedback-dialog.component';
import { UserNotificationService } from '../../services/user-notification.service';
import { ToastrNotification } from '../../../shared/services/toastr-service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, FeedbackDialog],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar implements OnInit {
  @ViewChild('feedbackDialog') feedbackDialog!: FeedbackDialog;
  sidebarCollapsed = false;
  menuItems: ReportModel[] | undefined;
  selectedReportId: string | null = null;

  unreadCount = 0;
  showNotifications = false;
  notifications: any[] = [];
  private notificationInterval: any;

  constructor(
    private router: Router,
    private data: Data,
    public themeService: ThemeService,
    private authService: AuthService,
    public layoutService: LayoutService,
    private notificationService: UserNotificationService,
    private toastr: ToastrNotification
  ) { }

  ngOnInit(): void {
    // 1. Load Menu
    this.data.GetMenuSidebar().subscribe(res => {
      this.menuItems = res;
    });

    // 2. Restore selected report state
    const reported = localStorage.getItem("selectedReport");
    if (reported) {
      const report = JSON.parse(reported);
      this.selectedReportId = report.id;
      // Navigate to the saved report
      this.router.navigate(['/report', report.id]);
    }

    // 3. Notification Polling
    this.checkNotifications();
    this.notificationInterval = setInterval(() => this.checkNotifications(), 60000); // 1 min poll
  }

  ngOnDestroy() {
    if (this.notificationInterval) {
      clearInterval(this.notificationInterval);
    }
  }

  toggleSidebar() {
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  selectMenu(item: ReportModel) {
    this.selectedReportId = item.id;
    localStorage.setItem("selectedReport", JSON.stringify(item));
    this.router.navigate(['/report', item.id]);
  }

  openFeedback() {
    this.feedbackDialog.open();
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.loadNotifications();
    }
  }

  loadNotifications() {
    this.notificationService.getMyNotifications().subscribe({
      next: (res) => {
        if (res.success) {
          this.notifications = res.data;
          // Recalculate unread locally
          this.unreadCount = this.notifications.filter(n => !n.isRead).length;
        }
      }
    });
  }

  onNotificationClick(item: any) {
    if (!item.isRead) {
      this.notificationService.markAsRead(item.id).subscribe(() => {
        item.isRead = true;
        this.unreadCount = Math.max(0, this.unreadCount - 1);
      });
    }

    if (item.type === 'Feedback') {
      this.openFeedback();
    }

    this.showNotifications = false;
  }

  checkNotifications() {
    this.notificationService.getUnreadCount().subscribe({
      next: (res) => {
        if (res && res.count !== undefined) {
          this.unreadCount = res.count;
        }
      },
      error: (err) => console.error('Notification check failed', err)
    });
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['login']);
  }

  get currentTheme(): string {
    const theme = this.themeService.theme();
    switch (theme) {
      case 'dark': return 'Ma trận (Dark)';
      case 'light': return 'Rực rỡ (Light)';
      case 'emerald': return 'Cyber Emerald';
      default: return 'Giao diện';
    }
  }

  get themeIcon(): string {
    const theme = this.themeService.theme();
    switch (theme) {
      case 'dark': return 'fa-solid fa-moon';
      case 'light': return 'fa-solid fa-sun';
      case 'emerald': return 'fa-solid fa-gem';
      default: return 'fa-solid fa-palette';
    }
  }
}

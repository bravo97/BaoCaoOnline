import { Component, OnInit, OnDestroy, ViewChild, ViewContainerRef, Type, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ReportModel } from '../../models/reportModel';
import { ReportRegistry } from '../../models/report.registry';
import { GenericReportTableComponent } from '../../components/generic-report-table/generic-report-table.component';
import { Data } from '../../services/data';

@Component({
  selector: 'app-report-viewer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="h-full w-full flex flex-col relative">
      <!-- Report Container - Always present to keep ViewContainerRef alive -->
      <div class="h-full w-full transition-opacity duration-300" [class.opacity-40]="isLoading">
        <ng-container #container></ng-container>
      </div>

      <!-- Loading Overlay with smooth fade -->
      <div *ngIf="isLoading" 
           class="absolute inset-0 flex items-center justify-center bg-background/80 backdrop-blur-md z-50 animate-in fade-in duration-200">
        <div class="flex flex-col items-center gap-4">
          <div class="w-12 h-12 border-4 border-accent/20 border-t-accent rounded-full animate-spin"></div>
          <p class="text-xs font-black uppercase tracking-[0.2em] text-accent animate-pulse">Đang tải...</p>
        </div>
      </div>

      <!-- Error Overlay -->
      <div *ngIf="error && !isLoading" 
           class="absolute inset-0 flex items-center justify-center bg-background/95 backdrop-blur-sm z-50 animate-in fade-in duration-300">
        <div class="glass-effect rounded-2xl p-8 border border-glass/40 text-center max-w-md">
          <i class="fa-solid fa-exclamation-triangle text-5xl text-red-500 mb-4"></i>
          <h3 class="text-lg font-black text-primary mb-2">Không thể tải báo cáo</h3>
          <p class="text-sm text-secondary">{{ error }}</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
      width: 100%;
    }
  `]
})
export class ReportViewer implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('container', { read: ViewContainerRef }) container?: ViewContainerRef;

  report?: ReportModel;
  isLoading = true;
  error: string | null = null;
  private destroy$ = new Subject<void>();
  private pendingReport?: ReportModel;

  constructor(
    private route: ActivatedRoute,
    private dataService: Data,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    // Subscribe to route params to get reportId
    this.route.params.pipe(
      takeUntil(this.destroy$)
    ).subscribe(params => {
      const reportId = params['id'];
      if (reportId) {
        this.loadReport(reportId);
      }
    });
  }

  ngAfterViewInit(): void {
    // If we have a pending report, load it now that the view is ready
    if (this.pendingReport) {
      this.loadComponent(this.pendingReport);
      this.pendingReport = undefined;
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadReport(reportId: string) {
    this.isLoading = true;
    this.error = null;

    // Get report metadata from sidebar data (or create a dedicated API call)
    this.dataService.GetMenuSidebar().subscribe({
      next: (reports) => {
        const foundReport = reports.find((r: ReportModel) => r.id === reportId);

        if (foundReport) {
          this.report = foundReport;

          // Try to load component immediately if view is ready
          if (this.container) {
            // Small delay to let loading overlay appear smoothly
            // This prevents jarring flash when switching reports
            setTimeout(() => {
              this.container!.clear();
              this.loadComponent(foundReport);
              this.isLoading = false;
              this.cdr.detectChanges();
            }, 100);
          } else {
            // Store for loading in AfterViewInit
            this.pendingReport = foundReport;
            this.isLoading = false;
          }
        } else {
          this.error = `Không tìm thấy báo cáo với ID: ${reportId}`;
          this.isLoading = false;
        }
      },
      error: (err) => {
        console.error('Error loading report:', err);
        this.error = 'Không thể kết nối đến server. Vui lòng thử lại sau.';
        this.isLoading = false;
      }
    });
  }

  private loadComponent(report: ReportModel) {
    if (!this.container) {
      console.error('ViewContainerRef not available');
      this.pendingReport = report;
      return;
    }

    // Get component from registry or fallback to generic table
    const componentType: Type<any> = ReportRegistry.getComponent(report.id) || GenericReportTableComponent;

    const componentRef = this.container.createComponent(componentType);

    // Pass the report input to the dynamic component
    if ('report' in componentRef.instance) {
      componentRef.instance.report = report;
    }

    this.cdr.detectChanges();
  }
}

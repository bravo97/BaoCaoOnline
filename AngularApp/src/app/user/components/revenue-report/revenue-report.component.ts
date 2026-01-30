import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportModel } from '../../models/reportModel';
import { Data } from '../../services/data';
import { ToastrNotification } from '../../../shared/services/toastr-service';

interface SummaryCard {
    title: string;
    value: string;
    change: string;
    icon: string;
    trend: 'up' | 'down' | 'neutral';
}

@Component({
    selector: 'app-revenue-report',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './revenue-report.component.html',
    styleUrl: './revenue-report.component.scss',
})
export class RevenueReportComponent implements OnInit {
    private _report?: ReportModel;

    @Input() set report(value: ReportModel | undefined) {
        this._report = value;
        if (value) {
            this.loadReportData();
        }
    }

    get report(): ReportModel | undefined {
        return this._report;
    }

    summaryCards: SummaryCard[] = [];
    chartData: any[] = [];
    detailedData: any[] = [];
    isLoading = false;

    fromDate: string = '';
    toDate: string = '';

    constructor(
        private service: Data,
        private toastr: ToastrNotification
    ) { }

    ngOnInit(): void {
        this.fromDate = '2025-01-01';
        this.toDate = new Date().toISOString().slice(0, 10);
    }

    loadReportData() {
        if (!this.report) return;

        this.isLoading = true;

        // Load actual data from API
        this.service.GetReportData(this.report.id).subscribe({
            next: (res) => {
                this.detailedData = res;
                this.calculateSummaryCards(res);
                this.prepareChartData(res);
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                this.toastr.error("Không thể lấy dữ liệu báo cáo");
                console.error(err);

                // Fallback to mock data for demo
                this.loadMockData();
            }
        });
    }

    loadMockData() {
        // Mock summary cards
        this.summaryCards = [
            {
                title: 'Tổng Doanh Thu',
                value: '2.5 tỷ',
                change: '+12.5%',
                icon: 'fa-dollar-sign',
                trend: 'up'
            },
            {
                title: 'Đơn Hàng',
                value: '1,234',
                change: '+8.2%',
                icon: 'fa-shopping-cart',
                trend: 'up'
            },
            {
                title: 'Giá Trị TB',
                value: '2.1 triệu',
                change: '-3.1%',
                icon: 'fa-chart-line',
                trend: 'down'
            }
        ];

        // Mock chart data
        this.chartData = [
            { month: 'T1', value: 180 },
            { month: 'T2', value: 220 },
            { month: 'T3', value: 195 },
            { month: 'T4', value: 280 },
            { month: 'T5', value: 310 },
            { month: 'T6', value: 265 }
        ];

        // Mock detailed data
        this.detailedData = [
            { period: 'Tháng 1', revenue: '180 triệu', orders: 156, avgValue: '1.15 triệu' },
            { period: 'Tháng 2', revenue: '220 triệu', orders: 189, avgValue: '1.16 triệu' },
            { period: 'Tháng 3', revenue: '195 triệu', orders: 167, avgValue: '1.17 triệu' },
            { period: 'Tháng 4', revenue: '280 triệu', orders: 234, avgValue: '1.20 triệu' },
            { period: 'Tháng 5', revenue: '310 triệu', orders: 267, avgValue: '1.16 triệu' },
            { period: 'Tháng 6', revenue: '265 triệu', orders: 221, avgValue: '1.20 triệu' }
        ];
    }

    calculateSummaryCards(data: any[]) {
        // Calculate real summary from data
        // This is a placeholder - adjust based on actual data structure
        const totalRevenue = data.reduce((sum, item) => sum + (parseFloat(item.revenue) || 0), 0);

        this.summaryCards = [
            {
                title: 'Tổng Doanh Thu',
                value: this.formatCurrency(totalRevenue),
                change: '+12.5%',
                icon: 'fa-dollar-sign',
                trend: 'up'
            },
            {
                title: 'Số Lượng',
                value: data.length.toString(),
                change: '+8.2%',
                icon: 'fa-list',
                trend: 'up'
            },
            {
                title: 'Trung Bình',
                value: this.formatCurrency(totalRevenue / data.length),
                change: '+5.1%',
                icon: 'fa-chart-line',
                trend: 'up'
            }
        ];
    }

    prepareChartData(data: any[]) {
        // Prepare chart data from actual data
        // This is a placeholder - adjust based on actual data structure
        this.chartData = data.slice(0, 12).map((item, index) => ({
            month: `T${index + 1}`,
            value: parseFloat(item.revenue) || 0
        }));
    }

    formatCurrency(value: number): string {
        if (value >= 1000000000) {
            return `${(value / 1000000000).toFixed(1)} tỷ`;
        } else if (value >= 1000000) {
            return `${(value / 1000000).toFixed(1)} triệu`;
        } else if (value >= 1000) {
            return `${(value / 1000).toFixed(1)}k`;
        }
        return value.toString();
    }

    applyFilter() {
        this.loadReportData();
    }

    getMaxChartValue(): number {
        return Math.max(...this.chartData.map(d => d.value));
    }

    getChartBarHeight(value: number): number {
        const max = this.getMaxChartValue();
        return max > 0 ? (value / max) * 100 : 0;
    }
}

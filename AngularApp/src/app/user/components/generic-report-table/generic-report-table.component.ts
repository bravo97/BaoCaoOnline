import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ColumReportModel, ReportModel, ReportParameter } from '../../models/reportModel';
import { Data } from '../../services/data';
import { ToastrNotification } from '../../../shared/services/toastr-service';
import { forkJoin } from 'rxjs';

@Component({
    selector: 'app-generic-report-table',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './generic-report-table.component.html',
    styleUrl: './generic-report-table.component.scss',
})
export class GenericReportTableComponent implements OnInit {
    private _report?: ReportModel;
    @Input() set report(value: ReportModel | undefined) {
        this._report = value;
        if (value) {
            this.resetData();
            this.loadReport(value);
        }
    }
    get report(): ReportModel | undefined {
        return this._report;
    }
    columns: ColumReportModel[] = [];
    reportID: string = '';
    data: any[] = [];
    pagedData: any[] = [];

    // Dynamic Parameters
    parameters: ReportParameter[] = [];
    isLoading = false;

    pageIndex = 0;
    pageSize = 50;
    pageSizeOptions = [10, 20, 50, 100, 200];
    totalPages = 0;

    showMobileFilter = false;

    constructor(private service: Data, private toastr: ToastrNotification) { }

    toggleMobileFilter() {
        this.showMobileFilter = !this.showMobileFilter;
    }

    ngOnInit(): void {
        // Init logic moved to loadReport
    }

    private formatDate(date: Date): string {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }


    private resetData() {
        this.columns = [];
        this.data = [];
        this.pagedData = [];
        this.pageIndex = 0;
        this.totalPages = 0;
        this.parameters = [];
    }

    loadReport(report: ReportModel) {
        this.isLoading = true;
        this.reportID = report.id;
        this.resetData(); // Ensure fresh state

        console.log(`Loading report ${report.id}...`);

        forkJoin({
            columns: this.service.GetReportDataColumn(report.id),
            params: this.service.GetReportParams(report.id)
        }).subscribe({
            next: ({ columns, params }: { columns: ColumReportModel[], params: any[] }) => {
                console.log('Report metadata loaded:', { columns, params });

                // 1. Setup Columns
                this.columns = columns;

                // 2. Setup Parameters
                this.parameters = params.map((p: any) => {
                    // Normalize keys to support both camelCase (JSON default) and PascalCase (C# raw)
                    const param = p.param || p.Param;
                    // Use ParamName for label as requested, fallback to Param if empty
                    const label = p.paramName || p.ParamName || p.param || p.Param || p.name || p.Name;

                    const dataParameter = p.dataParameter || p.DataParameter;
                    const query = p.query || p.Query;

                    const isSelect = dataParameter && dataParameter.length > 0;

                    // Heuristic for type
                    let type: any = 'text';
                    if (isSelect) type = 'select';
                    else if (param.toLowerCase().includes('ngay') || param.toLowerCase().includes('date')) type = 'date';
                    else if (param.toLowerCase().includes('check') || param.toLowerCase().includes('flag')) type = 'boolean';

                    // Prepare metadata for binding
                    let valueField = query?.columnValue || query?.ColumnValue;
                    let displayField = query?.columnDisplay || query?.ColumnDisplay;

                    // Normalize fields against actual data keys (Case Insensitive Support)
                    if (isSelect && dataParameter.length > 0) {
                        const firstRow = dataParameter[0];
                        const keys = Object.keys(firstRow);

                        if (valueField) {
                            const match = keys.find(k => k.toLowerCase() === valueField.toLowerCase());
                            if (match) valueField = match;
                        } else {
                            // Heuristic if no config: look for Id/Ma/Code/Key
                            valueField = keys.find(k => k.toLowerCase().match(/^(id|ma|code|key)$/)) || keys[0];
                        }

                        if (displayField) {
                            const match = keys.find(k => k.toLowerCase() === displayField.toLowerCase());
                            if (match) displayField = match;
                        } else {
                            // Heuristic if no config: look for Name/Ten/Label/Description
                            displayField = keys.find(k => k.toLowerCase().match(/^(name|ten|label|description|diengiai)$/)) || keys[1] || keys[0];
                        }
                    }

                    // Default value logic
                    // Default value logic
                    let value: any = undefined;
                    if (type === 'date') {
                        const now = new Date();
                        if (param.toLowerCase().includes('tu') || param.toLowerCase().includes('start')) {
                            value = this.formatDate(new Date(now.getFullYear(), 0, 1));
                        } else {
                            value = this.formatDate(now);
                        }
                    }
                    // Remove default selection for 'select' - default to null (All)

                    return {
                        key: param,
                        label: label,
                        type: type,
                        required: type !== 'select', // Select is optional (All)
                        value: value,
                        options: dataParameter,
                        valueField: valueField,
                        displayField: displayField
                    };
                });

                // Sort parameters: FromDate/ToDate first
                this.parameters.sort((a, b) => {
                    const getPriority = (p: ReportParameter) => {
                        const k = p.key.toLowerCase();
                        if (p.type === 'date' && (k.includes('tu') || k.includes('start') || k.includes('from'))) return 0;
                        if (p.type === 'date' && (k.includes('den') || k.includes('end') || k.includes('to'))) return 1;
                        return 10;
                    };
                    return getPriority(a) - getPriority(b);
                });

                this.isLoading = false;
            },
            error: (err: any) => {
                console.error('Error loading report metadata:', err);
                this.toastr.error("Không thể tải cấu hình báo cáo (Cột/Tham số)");
                this.isLoading = false;
                this.setDefaultLegacyParams();
            }
        });
    }

    private setDefaultLegacyParams() {
        const now = new Date();
        const startOfYear = new Date(now.getFullYear(), 0, 1);
        this.parameters = [
            { key: 'TuNgay', label: 'Từ Ngày', type: 'date', value: this.formatDate(startOfYear), required: true },
            { key: 'DenNgay', label: 'Đến Ngày', type: 'date', value: this.formatDate(now), required: true }
        ];
    }

    applyFilter() {
        // Construct payload from parameters
        const payload: any = {};

        let isValid = true;
        for (const param of this.parameters) {
            if (param.required && (param.value === undefined || param.value === null || param.value === '')) {
                this.toastr.warning(`Vui lòng nhập ${param.label}`);
                isValid = false;
                break;
            }
            payload[param.key] = (param.value === undefined || param.value === null) ? '' : param.value;
        }

        if (!isValid) return;

        // Don't clear data - keep old data visible during loading
        this.isLoading = true;

        this.service.GetReportData(this.reportID, payload).subscribe({
            next: (res) => {
                // Only update data after successful response
                this.data = res;
                this.pageIndex = 0;
                this.calculateTotalPages();
                this.refreshPagedData();
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                this.toastr.error("Không thể lấy dữ liệu báo cáo");
                console.error(err);
            }
        });
    }

    calculateTotalPages() {
        this.totalPages = Math.ceil(this.data.length / this.pageSize);
    }

    onPageSizeChange() {
        this.pageIndex = 0;
        this.calculateTotalPages();
        this.refreshPagedData();
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

    goToPage(index: number) {
        if (index >= 0 && index < this.totalPages) {
            this.pageIndex = index;
            this.refreshPagedData();
        }
    }

    getPageNumbers(): number[] {
        const pages = [];
        const maxVisible = 5;
        let start = Math.max(0, this.pageIndex - Math.floor(maxVisible / 2));
        let end = Math.min(this.totalPages, start + maxVisible);

        if (end - start < maxVisible) {
            start = Math.max(0, end - maxVisible);
        }

        for (let i = start; i < end; i++) {
            pages.push(i);
        }
        return pages;
    }

    getDisplayValue(opt: any, param: ReportParameter): string {
        if (!opt) return '';

        // Fallback names if fields not configured/found
        const valKey = param.valueField || (opt.Id ? 'Id' : Object.keys(opt)[0]);
        const dispKey = param.displayField || (opt.Name ? 'Name' : (opt.Ten ? 'Ten' : valKey));

        const val = opt[valKey];
        const disp = opt[dispKey];

        if (val === undefined || val === null) return disp || '';
        if (disp === undefined || disp === null) return val || '';

        const strVal = String(val).trim();
        const strDisp = String(disp).trim();

        // If display text already starts with value (e.g. "K01 - Kho Tong"), don't prefix again
        if (strDisp.toLowerCase().startsWith(strVal.toLowerCase())) {
            return strDisp;
        }

        return `${strVal} - ${strDisp}`;
    }
}

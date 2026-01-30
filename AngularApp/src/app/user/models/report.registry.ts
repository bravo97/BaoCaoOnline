import { Type } from '@angular/core';
import { RevenueReportComponent } from '../components/revenue-report/revenue-report.component';

export interface ReportComponentMapping {
    [key: string]: Type<any>;
}

export class ReportRegistry {
    private static registry: ReportComponentMapping = {};

    static register(type: string, component: Type<any>) {
        this.registry[type] = component;
    }

    static getComponent(type: string): Type<any> | undefined {
        return this.registry[type];
    }

    static getAll(): ReportComponentMapping {
        return { ...this.registry };
    }
}

// Register specific report components
// Note: You can register reports by their exact ID or by pattern matching
ReportRegistry.register('revenue-monthly', RevenueReportComponent);
ReportRegistry.register('revenue-yearly', RevenueReportComponent);
ReportRegistry.register('doanh-thu', RevenueReportComponent);


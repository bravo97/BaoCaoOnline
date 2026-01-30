// report-data.ts
export interface ReportParameter {
  key: string;       // e.g. "TuNgay", "Kho"
  label: string;     // e.g. "Từ Ngày", "Kho Hàng"
  type: 'text' | 'number' | 'date' | 'boolean' | 'select';
  value?: any;       // Default value
  placeholder?: string;
  required?: boolean;
  dataSource?: string; // Table name for lookup, e.g. "DM_Kho"
  options?: any[];     // Populated options
  valueField?: string;
  displayField?: string;
}

export interface ReportModel {
  id: string;
  name: string;
  fullName: string;
  group: string;
  parameters?: ReportParameter[];
}

export interface GroupReportModel {
  name: string;
  fullName: string;
}


export interface ColumReportModel {
  reportid: string;
  columnName: string;
  displayName: string;
  dataType: string;
  columnWidth?: number; // Width in pixels from API
}
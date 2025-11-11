// report-data.ts
export interface ReportModel {
  id: string;
  name: string;
  fullName: string;
  group: string;
}

export interface GroupReportModel{
  name:string;
  fullName:string;
}


export interface ColumReportModel {
  id: string;
  cloumnName: string;
  displayName: string;
  dataType: string;
}

// Danh sách báo cáo
export const REPORTS: ReportModel[] = [
  { id: 'r1', name: 'sales-summary', fullName: 'Báo cáo tổng quan doanh số', group: 'Doanh số' },
  { id: 'r2', name: 'monthly-sales', fullName: 'Báo cáo doanh số theo tháng', group: 'Doanh số' },
  { id: 'r3', name: 'customer-list', fullName: 'Danh sách khách hàng', group: 'Khách hàng' }
];

// Cột các báo cáo
export const REPORT_COLUMNS: ColumReportModel[] = [
  { id: 'c1', cloumnName: 'date', displayName: 'Ngày', dataType: 'date' },
  { id: 'c2', cloumnName: 'total', displayName: 'Tổng doanh số', dataType: 'number' },
  { id: 'c3', cloumnName: 'profit', displayName: 'Lợi nhuận', dataType: 'number' },
  { id: 'c4', cloumnName: 'customerName', displayName: 'Tên khách hàng', dataType: 'string' },
  { id: 'c5', cloumnName: 'email', displayName: 'Email', dataType: 'string' },
  { id: 'c6', cloumnName: 'phone', displayName: 'Điện thoại', dataType: 'string' }
];

// Dữ liệu bảng mẫu
export const REPORT_DATA: Record<string, any[]> = {
  'sales-summary': [
    { date: '2025-11-01', total: 1000, profit: 200 },
    { date: '2025-11-02', total: 1500, profit: 300 }
  ],
  'monthly-sales': [
    { date: '2025-11', total: 5000, profit: 1000 }
  ],
  'customer-list': [
    { customerName: 'Nguyen Van A', email: 'a@example.com', phone: '0123456789' },
    { customerName: 'Tran Thi B', email: 'b@example.com', phone: '0987654321' }
  ]
};

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
  reportid: string;
  cloumnName: string;
  displayName: string;
  dataType: string;
}
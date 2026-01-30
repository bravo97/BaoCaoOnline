import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ColumReportModel } from '../models/reportModel';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class Data {
  private apiUrl = `${environment.apiUrl}/report`;

  constructor(private http: HttpClient) { }

  GetMenuSidebar() {
    return this.http.get<any>(this.apiUrl);
  }

  GetReportDataColumn(id: string) {
    return this.http.get<ColumReportModel[]>(`${this.apiUrl + ''}/${id}`);
  }

  GetReportData(id: string, params: any = {}) {
    return this.http.post<any[]>(`${this.apiUrl}/data/${id}`, params);
  }

  GetLookupData(tableName: string) {
    return this.http.get<any[]>(`${this.apiUrl}/lookup/${tableName}`);
  }

  GetReportParams(reportId: string) {
    return this.http.get<any[]>(`${this.apiUrl}/param/${reportId}`);
  }
}

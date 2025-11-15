import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ColumReportModel } from '../models/reportModel';

@Injectable({
  providedIn: 'root',
})
export class Data {
  private apiUrl = 'http://localhost:5000/api/report';

  constructor(private http: HttpClient) {}

  GetMenuSidebar(){
    return this.http.get<any>(this.apiUrl);
  }

  GetReportDataColumn(id:string){
    return this.http.get<ColumReportModel[]>(`${this.apiUrl+''}/${id}`);
  }

  GetReportData(id:string){
    return this.http.get<any[]>(`${this.apiUrl+'/data'}/${id}`);
  }
}

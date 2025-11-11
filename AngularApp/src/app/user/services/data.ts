import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Data {
  private apiUrl = 'https://localhost:7023/api/report';

  constructor(private http: HttpClient) {}

  GetMenuSidebar(){
    return this.http.get<any>(this.apiUrl);
  }

  GetReportDataColumn(id:string){
    return this.http.get<any>(`${this.apiUrl+''}/${id}`);
  }
}

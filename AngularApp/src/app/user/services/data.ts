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

  // getAll(): Observable<CustomerModel[]> {
  //   return this.http.get<ApiResponse<CustomerModel[]>>(this.apiUrl).pipe(
  //     map(res => res.data) // ✅ chỉ lấy phần data
  //   );
  // }

  // getById(id: string): Observable<CustomerModel> {
  //   return this.http.get<ApiResponse<CustomerModel>>(`${this.apiUrl}/${id}`).pipe(
  //     map(res => res.data)
  //   );
  // }

  // create(customer: CustomerModel): Observable<ApiResponse<any>> {
  //   return this.http.post<ApiResponse<any>>(this.apiUrl, customer);
  // }

  // update(customer: CustomerModel): Observable<ApiResponse<CustomerModel>> {
  //   return this.http.put<ApiResponse<CustomerModel>>(`${this.apiUrl}/${customer.id}`, customer);
  // }

  // delete(id: string): Observable<ApiResponse<any>> {
  //   return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  // }
}

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ProductRequestDto } from '../swagger/model/productRequestDto';
import { ProductResponseDto } from '../swagger/model/productResponseDto';
import { ProductResponseDtoHttpResponse } from '../swagger/model/productResponseDtoHttpResponse';
import { ProductResponseDtoPagedResultHttpResponse } from '../swagger/model/productResponseDtoPagedResultHttpResponse';
import { ObjectHttpResponse } from '../swagger/model/objectHttpResponse';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${environment.baseUrl}products`;

  constructor(private http: HttpClient) { }

  getAll(page: number = 1, pageSize: number = 10, searchTerm?: string): Observable<ProductResponseDtoPagedResultHttpResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<ProductResponseDtoPagedResultHttpResponse>(this.apiUrl, { params });
  }

  getById(id: string): Observable<ProductResponseDtoHttpResponse> {
    return this.http.get<ProductResponseDtoHttpResponse>(`${this.apiUrl}/${id}`);
  }

  create(product: ProductRequestDto): Observable<ProductResponseDtoHttpResponse> {
    return this.http.post<ProductResponseDtoHttpResponse>(this.apiUrl, product);
  }

  update(id: string, product: ProductRequestDto): Observable<ProductResponseDtoHttpResponse> {
    return this.http.put<ProductResponseDtoHttpResponse>(`${this.apiUrl}/${id}`, product);
  }

  delete(id: string): Observable<ObjectHttpResponse> {
    return this.http.delete<ObjectHttpResponse>(`${this.apiUrl}/${id}`);
  }
}


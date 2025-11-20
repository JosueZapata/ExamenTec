import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CategoryRequestDto } from '../swagger/model/categoryRequestDto';
import { CategoryResponseDtoHttpResponse } from '../swagger/model/categoryResponseDtoHttpResponse';
import { CategoryResponseDtoPagedResultHttpResponse } from '../swagger/model/categoryResponseDtoPagedResultHttpResponse';
import { ObjectHttpResponse } from '../swagger/model/objectHttpResponse';
import { CategoryLookupDtoIEnumerableHttpResponse } from '../swagger/model/categoryLookupDtoIEnumerableHttpResponse';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl = `${environment.baseUrl}categories`;

  constructor(private http: HttpClient) { }

  getAll(page: number = 1, pageSize: number = 10, searchTerm?: string): Observable<CategoryResponseDtoPagedResultHttpResponse> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }

    return this.http.get<CategoryResponseDtoPagedResultHttpResponse>(this.apiUrl, { params });
  }

  getById(id: string): Observable<CategoryResponseDtoHttpResponse> {
    return this.http.get<CategoryResponseDtoHttpResponse>(`${this.apiUrl}/${id}`);
  }

  create(category: CategoryRequestDto): Observable<CategoryResponseDtoHttpResponse> {
    return this.http.post<CategoryResponseDtoHttpResponse>(this.apiUrl, category);
  }

  update(id: string, category: CategoryRequestDto): Observable<CategoryResponseDtoHttpResponse> {
    return this.http.put<CategoryResponseDtoHttpResponse>(`${this.apiUrl}/${id}`, category);
  }

  delete(id: string): Observable<ObjectHttpResponse> {
    return this.http.delete<ObjectHttpResponse>(`${this.apiUrl}/${id}`);
  }

  search(searchTerm: string, maxResults: number = 20): Observable<CategoryLookupDtoIEnumerableHttpResponse> {
    const params = new HttpParams()
      .set('searchTerm', searchTerm)
      .set('maxResults', maxResults.toString());

    return this.http.get<CategoryLookupDtoIEnumerableHttpResponse>(`${this.apiUrl}/search`, { params });
  }
}

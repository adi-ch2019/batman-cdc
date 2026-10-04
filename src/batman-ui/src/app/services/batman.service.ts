import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BatmanItem {
  id: string;
  name: string;
  lastModified: string;
}

@Injectable({ providedIn: 'root' })
export class BatmanService {
  private readonly apiUrl = '/api';

  constructor(private http: HttpClient) {}

  getItems(): Observable<BatmanItem[]> {
    return this.http.get<BatmanItem[]>(`${this.apiUrl}/items`);
  }

  createItem(name: string): Observable<BatmanItem> {
    return this.http.post<BatmanItem>(`${this.apiUrl}/items`, { name });
  }
}
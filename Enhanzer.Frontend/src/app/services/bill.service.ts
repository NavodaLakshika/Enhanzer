import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LocationDetail {
  location_Code: string;
  location_Name: string;
}

export interface BillItem {
  itemName: string;
  batch: string;
  standardCost: number;
  standardPrice: number;
  margin: number;
  qty: number;
  freeQty: number;
  discount: number;
  totalCost: number;
  totalSelling: number;
}

export interface PurchaseBill {
  id?: number;
  createdAt?: string;
  totalItems: number;
  totalQuantity: number;
  totalCost: number;
  totalSelling: number;
  items: BillItem[];
}

@Injectable({
  providedIn: 'root'
})
export class BillService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5294/api';

  constructor() { }

  getLocations(): Observable<LocationDetail[]> {
    return this.http.get<LocationDetail[]>(`${this.apiUrl}/Locations`);
  }

  saveBill(bill: PurchaseBill): Observable<PurchaseBill> {
    return this.http.post<PurchaseBill>(`${this.apiUrl}/PurchaseBill`, bill);
  }

  getBill(id: number): Observable<PurchaseBill> {
    return this.http.get<PurchaseBill>(`${this.apiUrl}/PurchaseBill/${id}`);
  }
}

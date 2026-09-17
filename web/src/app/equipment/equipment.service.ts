import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Equipment, Rental } from './equipment';

// Where the API listens; the http launch profile of EquipmentRental.Api.
export const API_URL = 'http://localhost:5078/api/equipment';

@Injectable({ providedIn: 'root' })
export class EquipmentService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<Equipment[]> {
    return this.http.get<Equipment[]>(API_URL);
  }

  get(id: number): Observable<Equipment> {
    return this.http.get<Equipment>(`${API_URL}/${id}`);
  }

  rent(id: number, days: number): Observable<Rental> {
    return this.http.post<Rental>(`${API_URL}/${id}/rentals`, { days });
  }

  cancel(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/${id}/rentals/current`);
  }
}

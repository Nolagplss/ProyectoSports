import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ReservationResponse } from '../models/ReservationResponse';
import { Observable } from 'rxjs';
import { ReservationFilter } from '../models/ReservationFilter';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  private readonly API_URL = 'http://localhost:7024/api/reservation';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getAllReservations(): Observable<ReservationResponse[]> {
    return this.http.get<ReservationResponse[]>(`${this.API_URL}/with-facilities`, {
    headers: this.getHeaders()
    });
  }

  filterReservations(filter: ReservationFilter): Observable<ReservationResponse[]> {
    let params = new HttpParams();
    
    if (filter.userId) {
      params = params.set('userId', filter.userId.toString());
    }
    if (filter.facilityType) {
      params = params.set('facilityType', filter.facilityType);
    }
    if (filter.facilityName) {
      params = params.set('facilityName', filter.facilityName);
    }
    if (filter.startDate) {
      params = params.set('startDate', filter.startDate);
    }
    if (filter.endDate) {
      params = params.set('endDate', filter.endDate);
    }

    return this.http.get<ReservationResponse[]>(`${this.API_URL}/filter`, {
      headers: this.getHeaders(),
      params: params
    });
  }

  deleteReservation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`, {
      headers: this.getHeaders()
    });
  }

  markAsNoShow(id: number): Observable<any> {
    return this.http.post(`${this.API_URL}/reservations/${id}/noshow`, {}, {
      headers: this.getHeaders()
    });
  }
}

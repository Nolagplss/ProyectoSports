import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { Observable } from 'rxjs';
import { FacilityDTO } from '../models/FacilityDTO';

@Injectable({
  providedIn: 'root'
})
export class FacilityService {

  private readonly API_URL = 'http://localhost:7024/api/facilities';


  constructor(private http: HttpClient, private authService: AuthService) {}

  private getHeaders(): HttpHeaders {
    const token = this.authService.getToken();
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getAllFacilities(): Observable<FacilityDTO[]> {
    return this.http.get<FacilityDTO[]>(`${this.API_URL}`, {
      headers: this.getHeaders()
    });
  }

  filterAvailableFacilities(
    type: string,
    date: string, // "YYYY-MM-DD"
    startTime: string, // "HH:mm"
    endTime: string // "HH:mm"
  ): Observable<FacilityDTO[]> {
    let params = new HttpParams()
      .set('type', type)
      .set('date', date)
      .set('startTime', startTime)
      .set('endTime', endTime);

    return this.http.get<FacilityDTO[]>(`${this.API_URL}/filter`, {
      headers: this.getHeaders(),
      params: params
    });
  }

}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { LoginDto } from '../models/LoginDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly API_URL = 'http://localhost:7024/api/auth';

  constructor(private http: HttpClient) {}

  login(credentials: LoginDto): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.API_URL}/login`, credentials)
      .pipe(
        tap(res => {
          // Guardar token
          localStorage.setItem('authToken', res.token);
          
          // Decodificar el token
          const decoded = jwtDecode<any>(res.token);

          console.log('Token decodificado:', decoded);

          
          // Extraer valores del token
          const userId = decoded.sub;
          const email = decoded.email;
          const userName = decoded.name;
          const role = decoded.role;
          
          // Guardar en localStorage
          localStorage.setItem('userId', userId);
          localStorage.setItem('email', email);
          localStorage.setItem('userName', userName);
          localStorage.setItem('userRole', role);
          
          // Log para debugging
          console.log('Usuario autenticado:', {
            userId,
            email,
            userName,
            role,
            token: res.token
          });
        })
      );
  }

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('userId');
    localStorage.removeItem('email');
    localStorage.removeItem('userName');
    localStorage.removeItem('userRole');
  }

  getToken(): string | null {
    return localStorage.getItem('authToken');
  }

  getUserId(): string | null {
    return localStorage.getItem('userId');
  }

  getUserName(): string | null {
    return localStorage.getItem('userName');
  }

  getUserEmail(): string | null {
    return localStorage.getItem('email');
  }

  getUserRole(): string | null {
    return localStorage.getItem('userRole');
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decoded = jwtDecode<any>(token);
      // Verificar si el token no ha expirado
      if (decoded.exp) {
        const currentTime = Math.floor(Date.now() / 1000);
        return decoded.exp > currentTime;
      }
      return true;
    } catch (error) {
      console.error('Error decodificando token:', error);
      return false;
    }
  }

  getPermissions(): string[] {
    const token = this.getToken();
    if (!token) return [];

    try {
      const decoded = jwtDecode<any>(token);
      const permissions = decoded.permission;
      
      // El claim permission puede ser un string o un array
      if (Array.isArray(permissions)) {
        return permissions;
      } else if (typeof permissions === 'string') {
        return [permissions];
      }
      return [];
    } catch (error) {
      console.error('Error extrayendo permisos:', error);
      return [];
    }
  }

  hasPermission(permissionCode: string): boolean {
    return this.getPermissions().includes(permissionCode);
  }
}
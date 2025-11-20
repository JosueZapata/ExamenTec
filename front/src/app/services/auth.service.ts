import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { LoginRequestDto } from '../swagger/model/loginRequestDto';
import { LoginResponseDtoHttpResponse } from '../swagger/model/loginResponseDtoHttpResponse';
import { LoginResponseDto } from '../swagger/model/loginResponseDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.baseUrl}account`;
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';
  
  private currentUserSubject = new BehaviorSubject<LoginResponseDto | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  login(email: string, password: string): Observable<LoginResponseDtoHttpResponse> {
    const loginRequest: LoginRequestDto = { email, password };
    
    return this.http.post<LoginResponseDtoHttpResponse>(`${this.apiUrl}/login`, loginRequest)
      .pipe(
        tap(response => {
          if (response.result && response.result.token && response.result.expiresAt) {
            this.setUser(response.result);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/iniciar-sesion']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getCurrentUser(): LoginResponseDto | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    const user = this.getCurrentUser();
    if (!user || !user.token) {
      return false;
    }
    
    if (user.expiresAt) {
      const expiresDate = new Date(user.expiresAt);
      const now = new Date();
      if (now > expiresDate) {
        this.logout();
        return false;
      }
    }
    
    return true;
  }

  private setUser(user: LoginResponseDto): void {
    if (user.token) {
      localStorage.setItem(this.TOKEN_KEY, user.token);
    }
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private getStoredUser(): LoginResponseDto | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (!userJson) {
      return null;
    }
    
    try {
      const user: LoginResponseDto = JSON.parse(userJson);
      
      if (user.expiresAt) {
        const expiresDate = new Date(user.expiresAt);
        const now = new Date();
        if (now > expiresDate) {
          this.logout();
          return null;
        }
      }
      
      return user;
    } catch {
      return null;
    }
  }
}


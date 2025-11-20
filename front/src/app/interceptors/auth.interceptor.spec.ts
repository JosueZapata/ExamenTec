import { TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { authInterceptor } from './auth.interceptor';
import { AuthService } from '../services/auth.service';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['getToken', 'logout']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        {
          provide: AuthService,
          useValue: authServiceSpy
        },
        {
          provide: Router,
          useValue: routerSpy
        }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('no debe agregar header Authorization para el endpoint de login', () => {
    authService.getToken.and.returnValue('test-token');

    http.post(`${environment.baseUrl}account/login`, {}).subscribe();

    const req = httpMock.expectOne(`${environment.baseUrl}account/login`);
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });

  it('debe agregar header Authorization cuando existe token', () => {
    authService.getToken.and.returnValue('test-token');

    http.get(`${environment.baseUrl}products`).subscribe();

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
    req.flush({});
  });

  it('no debe agregar header Authorization cuando no existe token', () => {
    authService.getToken.and.returnValue(null);

    http.get(`${environment.baseUrl}products`).subscribe();

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    expect(req.request.headers.has('Authorization')).toBeFalse();
    req.flush({});
  });

  it('debe cerrar sesión y redirigir al login en error 401', () => {
    authService.getToken.and.returnValue('test-token');

    http.get(`${environment.baseUrl}products`).subscribe({
      next: () => fail('debería haber fallado'),
      error: () => {
        expect(authService.logout).toHaveBeenCalled();
        expect(router.navigate).toHaveBeenCalledWith(['/iniciar-sesion']);
      }
    });

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    req.error(new ErrorEvent('Error'), { status: 401 });
  });

  it('debe redirigir a la página de acceso denegado en error 403', () => {
    authService.getToken.and.returnValue('test-token');

    http.get(`${environment.baseUrl}products`).subscribe({
      next: () => fail('debería haber fallado'),
      error: () => {
        expect(router.navigate).toHaveBeenCalledWith(['/acceso-denegado']);
      }
    });

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    req.error(new ErrorEvent('Error'), { status: 403 });
  });

  it('debe redirigir a la página de acceso denegado en error 403 incluso sin token', () => {
    authService.getToken.and.returnValue(null);

    http.get(`${environment.baseUrl}products`).subscribe({
      next: () => fail('debería haber fallado'),
      error: () => {
        expect(router.navigate).toHaveBeenCalledWith(['/acceso-denegado']);
      }
    });

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    req.error(new ErrorEvent('Error'), { status: 403 });
  });
});


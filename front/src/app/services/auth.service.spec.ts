import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { LoginResponseDto } from '../swagger/model/loginResponseDto';
import { LoginResponseDtoHttpResponse } from '../swagger/model/loginResponseDtoHttpResponse';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        {
          provide: Router,
          useValue: routerSpy
        }
      ]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('debe ser creado', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('debe hacer login exitosamente y almacenar datos del usuario', () => {
      const mockResponse: LoginResponseDtoHttpResponse = {
        result: {
          token: 'test-token',
          expiresAt: new Date(Date.now() + 3600000).toISOString(),
          role: 'Admin'
        } as LoginResponseDto,
        message: '',
        statusCode: 200,
        isError: false
      };

      service.login('test@test.com', 'password123').subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.token).toBe('test-token');
      });

      const req = httpMock.expectOne(`${environment.baseUrl}account/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email: 'test@test.com', password: 'password123' });
      req.flush(mockResponse);

      expect(service.getToken()).toBe('test-token');
      expect(service.getCurrentUser()?.role).toBe('Admin');
    });

    it('debe manejar error de login', () => {
      service.login('test@test.com', 'wrong').subscribe({
        next: () => fail('debería haber fallado'),
        error: (error) => {
          expect(error).toBeTruthy();
        }
      });

      const req = httpMock.expectOne(`${environment.baseUrl}account/login`);
      req.error(new ErrorEvent('Error'), { status: 401 });
    });
  });

  describe('logout', () => {
    it('debe limpiar token y datos del usuario y navegar al login', () => {
      localStorage.setItem('auth_token', 'test-token');
      localStorage.setItem('auth_user', JSON.stringify({ role: 'Admin' }));

      service.logout();

      expect(localStorage.getItem('auth_token')).toBeNull();
      expect(localStorage.getItem('auth_user')).toBeNull();
      expect(service.getCurrentUser()).toBeNull();
      expect(router.navigate).toHaveBeenCalledWith(['/iniciar-sesion']);
    });
  });

  describe('getToken', () => {
    it('debe retornar token desde localStorage', () => {
      localStorage.setItem('auth_token', 'test-token');
      expect(service.getToken()).toBe('test-token');
    });

    it('debe retornar null si no existe token', () => {
      expect(service.getToken()).toBeNull();
    });
  });

  describe('getCurrentUser', () => {
    it('debe retornar el usuario actual', () => {
      const mockUser: LoginResponseDto = {
        token: 'test-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        role: 'Admin'
      };
      localStorage.setItem('auth_user', JSON.stringify(mockUser));

      service.login('test@test.com', 'password').subscribe();

      const req = httpMock.expectOne(`${environment.baseUrl}account/login`);
      req.flush({ result: mockUser } as LoginResponseDtoHttpResponse);

      expect(service.getCurrentUser()?.role).toBe('Admin');
    });

    it('debe retornar null si no existe usuario', () => {
      expect(service.getCurrentUser()).toBeNull();
    });
  });

  describe('isAuthenticated', () => {
    it('debe retornar true si el usuario está autenticado con token válido', () => {
      const mockUser: LoginResponseDto = {
        token: 'test-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        role: 'Admin'
      };

      service.login('test@test.com', 'password').subscribe();

      const req = httpMock.expectOne(`${environment.baseUrl}account/login`);
      req.flush({ result: mockUser } as LoginResponseDtoHttpResponse);

      expect(service.isAuthenticated()).toBeTrue();
    });

    it('debe retornar false si el token está expirado', () => {
      const mockUser: LoginResponseDto = {
        token: 'test-token',
        expiresAt: new Date(Date.now() - 1000).toISOString(),
        role: 'Admin',
        email: 'test@test.com'
      };

      localStorage.setItem('auth_token', 'test-token');
      localStorage.setItem('auth_user', JSON.stringify(mockUser));
      
      service['currentUserSubject'].next(mockUser);

      expect(service.isAuthenticated()).toBeFalse();
      expect(router.navigate).toHaveBeenCalledWith(['/iniciar-sesion']);
    });

    it('debe retornar false si no existe usuario', () => {
      expect(service.isAuthenticated()).toBeFalse();
    });

    it('debe retornar false si no existe token', () => {
      localStorage.setItem('auth_user', JSON.stringify({ role: 'Admin' }));
      expect(service.isAuthenticated()).toBeFalse();
    });
  });

  describe('currentUser$', () => {
    it('debe emitir cambios de usuario', (done) => {
      const mockUser: LoginResponseDto = {
        token: 'test-token',
        expiresAt: new Date(Date.now() + 3600000).toISOString(),
        role: 'Admin'
      };

      service.currentUser$.subscribe(user => {
        if (user) {
          expect(user.role).toBe('Admin');
          done();
        }
      });

      service.login('test@test.com', 'password').subscribe();

      const req = httpMock.expectOne(`${environment.baseUrl}account/login`);
      req.flush({ result: mockUser } as LoginResponseDtoHttpResponse);
    });
  });
});


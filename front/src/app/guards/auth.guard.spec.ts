import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';

describe('authGuard', () => {
  let guard: typeof authGuard;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(() => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule],
      providers: [
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

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    guard = authGuard;
  });

  it('debe ser creado', () => {
    expect(guard).toBeTruthy();
  });

  it('debe permitir acceso cuando el usuario está autenticado', () => {
    authService.isAuthenticated.and.returnValue(true);
    
    TestBed.runInInjectionContext(() => {
      const result = guard({} as any, { url: '/productos' } as any);
      expect(result).toBeTrue();
      expect(authService.isAuthenticated).toHaveBeenCalled();
    });
  });

  it('debe redirigir al login cuando el usuario no está autenticado', () => {
    authService.isAuthenticated.and.returnValue(false);

    TestBed.runInInjectionContext(() => {
      const result = guard({} as any, { url: '/productos' } as any);

      expect(result).toBeFalse();
      expect(router.navigate).toHaveBeenCalledWith(['/iniciar-sesion'], {
        queryParams: { returnUrl: '/productos' }
      });
    });
  });

  it('debe usar /inicio como returnUrl cuando se accede a la ruta raíz', () => {
    authService.isAuthenticated.and.returnValue(false);

    TestBed.runInInjectionContext(() => {
      const result = guard({} as any, { url: '/' } as any);

      expect(result).toBeFalse();
      expect(router.navigate).toHaveBeenCalledWith(['/iniciar-sesion'], {
        queryParams: { returnUrl: '/inicio' }
      });
    });
  });
});


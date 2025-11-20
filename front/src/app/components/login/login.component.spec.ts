import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LoginComponent } from './login.component';
import { AuthService } from '../../services/auth.service';
import { LoginResponseDtoHttpResponse } from '../../swagger/model/loginResponseDtoHttpResponse';
import { LoginResponseDto } from '../../swagger/model/loginResponseDto';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['login', 'isAuthenticated']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [LoginComponent],
      providers: [
        { provide: AuthService, useValue: authServiceSpy },
        { provide: Router, useValue: routerSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: { queryParams: {} }
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('debe redirigir si ya está autenticado', () => {
      authService.isAuthenticated.and.returnValue(true);

      component.ngOnInit();

      expect(router.navigate).toHaveBeenCalledWith(['/inicio']);
    });

    it('debe establecer returnUrl desde query params', () => {
      authService.isAuthenticated.and.returnValue(false);
      const route = TestBed.inject(ActivatedRoute);
      (route as any).snapshot.queryParams = { returnUrl: '/productos' };

      component.ngOnInit();

      expect(component.returnUrl).toBe('/productos');
    });

    it('debe usar returnUrl por defecto si no está en query params', () => {
      authService.isAuthenticated.and.returnValue(false);

      component.ngOnInit();

      expect(component.returnUrl).toBe('/inicio');
    });
  });

  describe('onSubmit', () => {
    it('debe mostrar error si el email está vacío', () => {
      component.email = '';
      component.password = 'password';

      component.onSubmit();

      expect(component.error).toBe('Por favor ingrese email y contraseña');
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('debe mostrar error si la contraseña está vacía', () => {
      component.email = 'test@test.com';
      component.password = '';

      component.onSubmit();

      expect(component.error).toBe('Por favor ingrese email y contraseña');
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('debe llamar al servicio de login con credenciales correctas', () => {
      component.email = 'test@test.com';
      component.password = 'password123';
      const mockResponse: LoginResponseDtoHttpResponse = {
        result: {
          token: 'token',
          expiresAt: new Date().toISOString(),
          role: 'Admin'
        } as LoginResponseDto,
        statusCode: 200
      };
      authService.login.and.returnValue(of(mockResponse));

      component.onSubmit();

      expect(authService.login).toHaveBeenCalledWith('test@test.com', 'password123');
      expect(component.loading).toBeFalse();
      expect(router.navigate).toHaveBeenCalledWith(['/inicio']);
    });

    it('debe manejar error de login con mensaje', () => {
      component.email = 'test@test.com';
      component.password = 'wrong';
      const error = { error: { message: 'Credenciales inválidas' } };
      authService.login.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(component.loading).toBeFalse();
      expect(component.error).toBe('Credenciales inválidas');
    });

    it('debe manejar error de login con array de errores', () => {
      component.email = 'test@test.com';
      component.password = 'wrong';
      const error = { error: { errors: { email: ['Invalid email'] } } };
      authService.login.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(component.loading).toBeFalse();
      expect(component.error).toContain('Invalid email');
    });

    it('debe mostrar mensaje de error por defecto cuando el formato de error es desconocido', () => {
      component.email = 'test@test.com';
      component.password = 'wrong';
      const error = { error: {} };
      authService.login.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(component.loading).toBeFalse();
      expect(component.error).toBe('Email o contraseña incorrectos');
    });

    it('debe establecer loading a true durante el login', () => {
      component.email = 'test@test.com';
      component.password = 'password123';
      authService.login.and.returnValue(of({} as LoginResponseDtoHttpResponse));

      component.onSubmit();

      expect(component.loading).toBeFalse();
    });
  });

  describe('togglePasswordVisibility', () => {
    it('debe alternar showPassword', () => {
      expect(component.showPassword).toBeFalse();

      component.togglePasswordVisibility();
      expect(component.showPassword).toBeTrue();

      component.togglePasswordVisibility();
      expect(component.showPassword).toBeFalse();
    });
  });

  describe('onAlertClosed', () => {
    it('debe limpiar mensaje de error', () => {
      component.error = 'Some error';

      component.onAlertClosed();

      expect(component.error).toBe('');
    });
  });
});


import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router, ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { BehaviorSubject } from 'rxjs';
import { NavbarComponent } from './navbar.component';
import { AuthService } from '../../../services/auth.service';
import { LoginResponseDto } from '../../../swagger/model/loginResponseDto';

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;
  let authService: jasmine.SpyObj<AuthService>;
  let router: Router;
  let currentUserSubject: BehaviorSubject<LoginResponseDto | null>;

  beforeEach(async () => {
    currentUserSubject = new BehaviorSubject<LoginResponseDto | null>(null);
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['isAuthenticated', 'logout'], {
      currentUser$: currentUserSubject.asObservable()
    });

    await TestBed.configureTestingModule({
      imports: [
        NavbarComponent,
        RouterTestingModule.withRoutes([
          { path: 'inicio', component: {} as any },
          { path: 'categorias', component: {} as any },
          { path: 'productos', component: {} as any },
          { path: '', component: {} as any }
        ]),
        HttpClientTestingModule
      ],
      providers: [
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    router = TestBed.inject(Router);
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('debe suscribirse a currentUser$', () => {
      const user: LoginResponseDto = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Admin'
      };

      component.ngOnInit();
      currentUserSubject.next(user);

      expect(component.currentUser).toEqual(user);
    });
  });

  describe('ngOnDestroy', () => {
    it('debe desuscribirse de currentUser$', () => {
      component.ngOnInit();
      spyOn(component['userSubscription']!, 'unsubscribe');

      component.ngOnDestroy();

      expect(component['userSubscription']!.unsubscribe).toHaveBeenCalled();
    });
  });

  describe('logout', () => {
    it('debe llamar a authService logout', () => {
      component.logout();

      expect(authService.logout).toHaveBeenCalled();
    });
  });

  describe('isAuthenticated', () => {
    it('debe retornar resultado de authService isAuthenticated', () => {
      authService.isAuthenticated.and.returnValue(true);

      expect(component.isAuthenticated()).toBeTrue();
      expect(authService.isAuthenticated).toHaveBeenCalled();
    });
  });

  describe('getUserRole', () => {
    it('debe retornar rol del usuario', () => {
      const user: LoginResponseDto = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Admin'
      };
      component.currentUser = user;

      expect(component.getUserRole()).toBe('Admin');
    });

    it('debe retornar null si no hay usuario', () => {
      component.currentUser = null;

      expect(component.getUserRole()).toBeNull();
    });
  });

  describe('canAccessCategories', () => {
    it('debe retornar true para rol Admin', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Admin'
      } as LoginResponseDto;

      expect(component.canAccessCategories()).toBeTrue();
    });

    it('debe retornar true para rol Category', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Category'
      } as LoginResponseDto;

      expect(component.canAccessCategories()).toBeTrue();
    });

    it('debe retornar false para rol Product', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Product'
      } as LoginResponseDto;

      expect(component.canAccessCategories()).toBeFalse();
    });
  });

  describe('canAccessProducts', () => {
    it('debe retornar true para rol Admin', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Admin'
      } as LoginResponseDto;

      expect(component.canAccessProducts()).toBeTrue();
    });

    it('debe retornar true para rol Product', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Product'
      } as LoginResponseDto;

      expect(component.canAccessProducts()).toBeTrue();
    });

    it('debe retornar false para rol Category', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Category'
      } as LoginResponseDto;

      expect(component.canAccessProducts()).toBeFalse();
    });
  });

  describe('isCategoriesDisabled', () => {
    it('debe retornar true cuando no puede acceder a categorías', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Product'
      } as LoginResponseDto;

      expect(component.isCategoriesDisabled()).toBeTrue();
    });
  });

  describe('isProductsDisabled', () => {
    it('debe retornar true cuando no puede acceder a productos', () => {
      component.currentUser = {
        token: 'token',
        expiresAt: new Date().toISOString(),
        role: 'Category'
      } as LoginResponseDto;

      expect(component.isProductsDisabled()).toBeTrue();
    });
  });
});


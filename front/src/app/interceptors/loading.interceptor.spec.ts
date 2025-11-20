import { TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { HttpClient } from '@angular/common/http';
import { fakeAsync, tick } from '@angular/core/testing';
import { loadingInterceptor } from './loading.interceptor';
import { LoadingService } from '../services/loading.service';
import { environment } from '../../environments/environment';

describe('loadingInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let loadingService: LoadingService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([loadingInterceptor])),
        provideHttpClientTesting(),
        LoadingService
      ]
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    loadingService = TestBed.inject(LoadingService);
  });

  afterEach(() => {
    httpMock.verify();
    loadingService.reset();
  });

  it('debe mostrar loading para peticiones API', fakeAsync(() => {
    let loadingState = false;
    loadingService.loading$.subscribe(state => {
      loadingState = state;
    });

    http.get(`${environment.baseUrl}products`).subscribe();

    tick(100);
    expect(loadingState).toBeTrue();

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    req.flush({});
    expect(loadingState).toBeFalse();
  }));

  it('no debe mostrar loading para peticiones que no son API', fakeAsync(() => {
    let loadingState = false;
    loadingService.loading$.subscribe(state => {
      loadingState = state;
    });

    http.get('https://api.external.com/data').subscribe();

    tick(100);
    expect(loadingState).toBeFalse();

    const req = httpMock.expectOne('https://api.external.com/data');
    req.flush({});
  }));

  it('debe ocultar loading después de que la petición se complete', fakeAsync(() => {
    let loadingState = false;
    loadingService.loading$.subscribe(state => {
      loadingState = state;
    });

    http.get(`${environment.baseUrl}products`).subscribe();

    tick(100);
    expect(loadingState).toBeTrue();

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    req.flush({});
    expect(loadingState).toBeFalse();
  }));

  it('debe ocultar loading después de que la petición falle', fakeAsync(() => {
    let loadingState = false;
    loadingService.loading$.subscribe(state => {
      loadingState = state;
    });

    http.get(`${environment.baseUrl}products`).subscribe({
      next: () => fail('debería haber fallado'),
      error: () => {}
    });

    tick(100);
    expect(loadingState).toBeTrue();

    const req = httpMock.expectOne(`${environment.baseUrl}products`);
    req.error(new ErrorEvent('Error'));
    expect(loadingState).toBeFalse();
  }));
});


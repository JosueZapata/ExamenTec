import { TestBed } from '@angular/core/testing';
import { LoadingService } from './loading.service';
import { fakeAsync, tick } from '@angular/core/testing';

describe('LoadingService', () => {
  let service: LoadingService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoadingService);
  });

  it('debe ser creado', () => {
    expect(service).toBeTruthy();
  });

  describe('show', () => {
    it('debe mostrar loading después del delay', fakeAsync(() => {
      let loadingState = false;
      service.loading$.subscribe(state => {
        loadingState = state;
      });

      service.show();
      expect(loadingState).toBeFalse();

      tick(100);
      expect(loadingState).toBeTrue();
    }));

    it('debe incrementar el contador de peticiones', fakeAsync(() => {
      service.show();
      service.show();
      service.show();

      let loadingState = false;
      service.loading$.subscribe(state => {
        loadingState = state;
      });

      tick(100);
      expect(loadingState).toBeTrue();
    }));
  });

  describe('hide', () => {
    it('debe ocultar loading cuando el contador de peticiones llega a cero', fakeAsync(() => {
      let loadingState = false;
      service.loading$.subscribe(state => {
        loadingState = state;
      });

      service.show();
      tick(100);
      expect(loadingState).toBeTrue();

      service.hide();
      expect(loadingState).toBeFalse();
    }));

    it('no debe ocultar loading si el contador de peticiones es mayor que cero', fakeAsync(() => {
      let loadingState = false;
      service.loading$.subscribe(state => {
        loadingState = state;
      });

      service.show();
      service.show();
      tick(100);
      expect(loadingState).toBeTrue();

      service.hide();
      expect(loadingState).toBeTrue();

      service.hide();
      expect(loadingState).toBeFalse();
    }));

    it('debe cancelar el timeout cuando hide se llama antes del delay', fakeAsync(() => {
      let loadingState = false;
      service.loading$.subscribe(state => {
        loadingState = state;
      });

      service.show();
      service.hide();

      tick(100);
      expect(loadingState).toBeFalse();
    }));
  });

  describe('reset', () => {
    it('debe resetear el estado de loading inmediatamente', fakeAsync(() => {
      let loadingState = false;
      service.loading$.subscribe(state => {
        loadingState = state;
      });

      service.show();
      service.show();
      tick(100);
      expect(loadingState).toBeTrue();

      service.reset();
      expect(loadingState).toBeFalse();
    }));

    it('debe cancelar el timeout pendiente', fakeAsync(() => {
      let loadingState = false;
      service.loading$.subscribe(state => {
        loadingState = state;
      });

      service.show();
      service.reset();

      tick(100);
      expect(loadingState).toBeFalse();
    }));
  });

  describe('loading$', () => {
    it('debe emitir el valor inicial de false', (done) => {
      service.loading$.subscribe(state => {
        expect(state).toBeFalse();
        done();
      });
    });

    it('debe emitir cambios en el estado de loading', fakeAsync(() => {
      const states: boolean[] = [];
      service.loading$.subscribe(state => {
        states.push(state);
      });

      service.show();
      tick(100);
      service.hide();

      expect(states).toContain(false);
      expect(states).toContain(true);
    }));
  });
});


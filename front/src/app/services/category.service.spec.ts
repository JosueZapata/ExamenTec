import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CategoryService } from './category.service';
import { environment } from '../../environments/environment';
import { CategoryRequestDto } from '../swagger/model/categoryRequestDto';

describe('CategoryService', () => {
  let service: CategoryService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CategoryService]
    });
    service = TestBed.inject(CategoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debe ser creado', () => {
    expect(service).toBeTruthy();
  });

  describe('getAll', () => {
    it('debe retornar categorías con paginación', () => {
      const mockResponse = {
        result: {
          items: [],
          page: 1,
          pageSize: 10,
          totalCount: 0
        },
        statusCode: 200
      };

      service.getAll(1, 10).subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.page).toBe(1);
      });

      const req = httpMock.expectOne(`${environment.baseUrl}categories?page=1&pageSize=10`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    it('debe incluir searchTerm en query params', () => {
      const mockResponse = {
        result: {
          items: [],
          page: 1,
          pageSize: 10,
          totalCount: 0
        },
        statusCode: 200
      };

      service.getAll(1, 10, 'test').subscribe();

      const req = httpMock.expectOne(`${environment.baseUrl}categories?page=1&pageSize=10&searchTerm=test`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });
  });

  describe('getById', () => {
    it('debe retornar categoría por id', () => {
      const categoryId = '123';
      const mockResponse = {
        result: {
          id: categoryId,
          name: 'Categoría Test'
        },
        statusCode: 200
      };

      service.getById(categoryId).subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.id).toBe(categoryId);
      });

      const req = httpMock.expectOne(`${environment.baseUrl}categories/${categoryId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });
  });

  describe('create', () => {
    it('debe crear una nueva categoría', () => {
      const category: CategoryRequestDto = {
        name: 'Nueva Categoría',
        description: 'Descripción'
      };

      const mockResponse = {
        result: {
          id: 'new-id',
          name: 'Nueva Categoría'
        },
        statusCode: 201
      };

      service.create(category).subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.name).toBe('Nueva Categoría');
      });

      const req = httpMock.expectOne(`${environment.baseUrl}categories`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(category);
      req.flush(mockResponse);
    });
  });

  describe('update', () => {
    it('debe actualizar una categoría existente', () => {
      const categoryId = '123';
      const category: CategoryRequestDto = {
        name: 'Categoría Actualizada',
        description: 'Nueva descripción'
      };

      const mockResponse = {
        result: {
          id: categoryId,
          name: 'Categoría Actualizada'
        },
        statusCode: 200
      };

      service.update(categoryId, category).subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.name).toBe('Categoría Actualizada');
      });

      const req = httpMock.expectOne(`${environment.baseUrl}categories/${categoryId}`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(category);
      req.flush(mockResponse);
    });
  });

  describe('delete', () => {
    it('debe eliminar una categoría', () => {
      const categoryId = '123';
      const mockResponse = {
        result: null,
        statusCode: 204
      };

      service.delete(categoryId).subscribe(response => {
        expect(response.statusCode).toBe(204);
      });

      const req = httpMock.expectOne(`${environment.baseUrl}categories/${categoryId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush(mockResponse);
    });
  });

  describe('search', () => {
    it('debe buscar categorías por término', () => {
      const searchTerm = 'test';
      const maxResults = 20;
      const mockResponse = {
        result: [],
        statusCode: 200
      };

      service.search(searchTerm, maxResults).subscribe(response => {
        expect(response.result).toBeTruthy();
      });

      const req = httpMock.expectOne(`${environment.baseUrl}categories/search?searchTerm=test&maxResults=20`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });

    it('debe usar maxResults por defecto si no se proporciona', () => {
      const searchTerm = 'test';
      const mockResponse = {
        result: [],
        statusCode: 200
      };

      service.search(searchTerm).subscribe();

      const req = httpMock.expectOne(`${environment.baseUrl}categories/search?searchTerm=test&maxResults=20`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });
  });
});


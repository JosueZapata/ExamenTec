import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProductService } from './product.service';
import { environment } from '../../environments/environment';
import { ProductRequestDto } from '../swagger/model/productRequestDto';
import { ProductResponseDto } from '../swagger/model/productResponseDto';

describe('ProductService', () => {
  let service: ProductService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductService]
    });
    service = TestBed.inject(ProductService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('debe ser creado', () => {
    expect(service).toBeTruthy();
  });

  describe('getAll', () => {
    it('debe retornar productos con paginación', () => {
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
        expect(response.result?.pageSize).toBe(10);
      });

      const req = httpMock.expectOne(`${environment.baseUrl}products?page=1&pageSize=10`);
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

      const req = httpMock.expectOne(`${environment.baseUrl}products?page=1&pageSize=10&searchTerm=test`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });
  });

  describe('getById', () => {
    it('debe retornar producto por id', () => {
      const productId = '123';
      const mockResponse = {
        result: {
          id: productId,
          name: 'Producto Test',
          price: 100
        } as ProductResponseDto,
        statusCode: 200
      };

      service.getById(productId).subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.id).toBe(productId);
      });

      const req = httpMock.expectOne(`${environment.baseUrl}products/${productId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);
    });
  });

  describe('create', () => {
    it('debe crear un nuevo producto', () => {
      const product: ProductRequestDto = {
        name: 'Nuevo Producto',
        price: 100,
        stock: 10,
        categoryId: 'category-id'
      };

      const mockResponse = {
        result: {
          id: 'new-id',
          name: 'Nuevo Producto',
          price: 100
        } as ProductResponseDto,
        statusCode: 201
      };

      service.create(product).subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.name).toBe('Nuevo Producto');
      });

      const req = httpMock.expectOne(`${environment.baseUrl}products`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(product);
      req.flush(mockResponse);
    });
  });

  describe('update', () => {
    it('debe actualizar un producto existente', () => {
      const productId = '123';
      const product: ProductRequestDto = {
        name: 'Producto Actualizado',
        price: 150,
        stock: 20,
        categoryId: 'category-id'
      };

      const mockResponse = {
        result: {
          id: productId,
          name: 'Producto Actualizado',
          price: 150
        } as ProductResponseDto,
        statusCode: 200
      };

      service.update(productId, product).subscribe(response => {
        expect(response.result).toBeTruthy();
        expect(response.result?.name).toBe('Producto Actualizado');
      });

      const req = httpMock.expectOne(`${environment.baseUrl}products/${productId}`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(product);
      req.flush(mockResponse);
    });
  });

  describe('delete', () => {
    it('debe eliminar un producto', () => {
      const productId = '123';
      const mockResponse = {
        result: null,
        statusCode: 204
      };

      service.delete(productId).subscribe(response => {
        expect(response.statusCode).toBe(204);
      });

      const req = httpMock.expectOne(`${environment.baseUrl}products/${productId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush(mockResponse);
    });
  });
});


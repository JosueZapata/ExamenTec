import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { ProductsComponent } from './products.component';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { ProductResponseDto } from '../../swagger/model/productResponseDto';
import { ProductResponseDtoPagedResult } from '../../swagger/model/productResponseDtoPagedResult';

describe('ProductsComponent', () => {
  let component: ProductsComponent;
  let fixture: ComponentFixture<ProductsComponent>;
  let productService: jasmine.SpyObj<ProductService>;

  const mockProducts: ProductResponseDto[] = [
    { id: '1', name: 'Producto 1', price: 100, stock: 10 } as ProductResponseDto,
    { id: '2', name: 'Producto 2', price: 200, stock: 20 } as ProductResponseDto
  ];

  const mockPagedResult: ProductResponseDtoPagedResult = {
    items: mockProducts,
    page: 1,
    pageSize: 10,
    totalCount: 2,
    totalPages: 1,
    hasPreviousPage: false,
    hasNextPage: false
  };

  beforeEach(async () => {
    const productServiceSpy = jasmine.createSpyObj('ProductService', ['getAll', 'delete']);
    const categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['search']);

    await TestBed.configureTestingModule({
      imports: [ProductsComponent, HttpClientTestingModule],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: CategoryService, useValue: categoryServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductsComponent);
    component = fixture.componentInstance;
    productService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('debe cargar productos al inicializar', () => {
      productService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.ngOnInit();

      expect(productService.getAll).toHaveBeenCalledWith(1, 10, undefined);
      expect(component.products).toEqual(mockProducts);
    });
  });

  describe('loadProducts', () => {
    it('debe cargar productos exitosamente', () => {
      productService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.loadProducts();

      expect(component.products).toEqual(mockProducts);
      expect(component.pagedResult).toEqual(mockPagedResult);
      expect(component.error).toBe('');
    });

    it('debe manejar error al cargar productos', () => {
      productService.getAll.and.returnValue(throwError(() => new Error('Error')));

      component.loadProducts();

      expect(component.error).toBe('Error al cargar los productos');
    });

    it('debe incluir searchTerm cuando se proporciona', () => {
      component.searchTerm = 'test';
      productService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.loadProducts();

      expect(productService.getAll).toHaveBeenCalledWith(1, 10, 'test');
    });
  });

  describe('search', () => {
    it('debe reiniciar página y cargar productos', () => {
      component.currentPage = 3;
      component.searchTerm = 'test';
      productService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.search();

      expect(component.currentPage).toBe(1);
      expect(productService.getAll).toHaveBeenCalledWith(1, 10, 'test');
    });
  });

  describe('goToPage', () => {
    it('debe navegar a una página válida', () => {
      component.pagedResult = mockPagedResult;
      productService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.goToPage(1);

      expect(component.currentPage).toBe(1);
      expect(productService.getAll).toHaveBeenCalled();
    });

    it('no debe navegar a una página inválida', () => {
      component.pagedResult = mockPagedResult;
      const initialPage = component.currentPage;

      component.goToPage(0);
      expect(component.currentPage).toBe(initialPage);

      component.goToPage(99);
      expect(component.currentPage).toBe(initialPage);
    });

    it('no debe navegar cuando pagedResult es null', () => {
      component.pagedResult = null;
      const initialPage = component.currentPage;

      component.goToPage(2);

      expect(component.currentPage).toBe(initialPage);
    });
  });

  describe('openCreateForm', () => {
    it('debe abrir formulario para crear', () => {
      component.openCreateForm();

      expect(component.isEditing).toBeFalse();
      expect(component.selectedProduct).toBeNull();
      expect(component.showForm).toBeTrue();
      expect(component.error).toBe('');
    });
  });

  describe('openEditForm', () => {
    it('debe abrir formulario para editar', () => {
      const product = mockProducts[0];

      component.openEditForm(product);

      expect(component.isEditing).toBeTrue();
      expect(component.selectedProduct).toEqual(product);
      expect(component.showForm).toBeTrue();
      expect(component.error).toBe('');
    });
  });

  describe('closeForm', () => {
    it('debe cerrar formulario y resetear estado', () => {
      component.showForm = true;
      component.isEditing = true;
      component.selectedProduct = mockProducts[0];
      component.error = 'Error';

      component.closeForm();

      expect(component.showForm).toBeFalse();
      expect(component.isEditing).toBeFalse();
      expect(component.selectedProduct).toBeNull();
      expect(component.error).toBe('');
    });
  });

  describe('onProductSaved', () => {
    it('debe recargar productos y cerrar formulario', () => {
      component.showForm = true;
      productService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.onProductSaved();

      expect(productService.getAll).toHaveBeenCalled();
      expect(component.showForm).toBeFalse();
    });
  });

  describe('openDeleteDialog', () => {
    it('debe abrir diálogo de eliminación con producto', () => {
      const product = mockProducts[0];

      component.openDeleteDialog(product);

      expect(component.productToDelete).toEqual(product);
      expect(component.showDeleteDialog).toBeTrue();
    });
  });

  describe('onConfirmDelete', () => {
    it('debe eliminar producto exitosamente', () => {
      component.productToDelete = mockProducts[0];
      productService.delete.and.returnValue(of({} as any));
      productService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.onConfirmDelete();

      expect(productService.delete).toHaveBeenCalledWith('1');
      expect(productService.getAll).toHaveBeenCalled();
      expect(component.showDeleteDialog).toBeFalse();
      expect(component.productToDelete).toBeNull();
    });

    it('debe manejar error al eliminar', () => {
      component.productToDelete = mockProducts[0];
      productService.delete.and.returnValue(throwError(() => ({ error: { message: 'Error al eliminar' } })));

      component.onConfirmDelete();

      expect(component.error).toBe('Error al eliminar');
      expect(component.showDeleteDialog).toBeFalse();
    });

    it('no debe eliminar si productToDelete es null', () => {
      component.productToDelete = null;

      component.onConfirmDelete();

      expect(productService.delete).not.toHaveBeenCalled();
    });
  });

  describe('onCancelDelete', () => {
    it('debe cerrar diálogo de eliminación', () => {
      component.showDeleteDialog = true;
      component.productToDelete = mockProducts[0];

      component.onCancelDelete();

      expect(component.showDeleteDialog).toBeFalse();
      expect(component.productToDelete).toBeNull();
    });
  });

  describe('getters', () => {
    it('debe retornar totalPages desde pagedResult', () => {
      component.pagedResult = mockPagedResult;
      expect(component.totalPages).toBe(1);
    });

    it('debe retornar 1 cuando pagedResult es null', () => {
      component.pagedResult = null;
      expect(component.totalPages).toBe(1);
    });

    it('debe retornar hasPreviousPage correctamente', () => {
      component.pagedResult = mockPagedResult;
      expect(component.hasPreviousPage).toBeFalse();

      component.pagedResult = { ...mockPagedResult, hasPreviousPage: true };
      expect(component.hasPreviousPage).toBeTrue();
    });

    it('debe retornar hasNextPage correctamente', () => {
      component.pagedResult = mockPagedResult;
      expect(component.hasNextPage).toBeFalse();

      component.pagedResult = { ...mockPagedResult, hasNextPage: true };
      expect(component.hasNextPage).toBeTrue();
    });
  });

  describe('deleteConfirmMessage', () => {
    it('debe retornar mensaje con nombre del producto', () => {
      component.productToDelete = mockProducts[0];
      expect(component.deleteConfirmMessage).toContain('Producto 1');
    });

    it('debe retornar mensaje genérico cuando el producto no tiene nombre', () => {
      component.productToDelete = { id: '1' } as ProductResponseDto;
      expect(component.deleteConfirmMessage).toContain('este producto');
    });
  });

  describe('formatPrice', () => {
    it('debe formatear precio correctamente', () => {
      expect(component.formatPrice(100)).toContain('100');
    });

    it('debe retornar "-" para precio undefined', () => {
      expect(component.formatPrice(undefined)).toBe('-');
    });

    it('debe retornar "-" para precio null', () => {
      expect(component.formatPrice(null as any)).toBe('-');
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


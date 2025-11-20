import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { ProductFormComponent } from './product-form.component';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { ProductResponseDto } from '../../swagger/model/productResponseDto';
import { CategoryLookupDto } from '../../swagger/model/categoryLookupDto';
import { HttpErrorResponse } from '@angular/common/http';

describe('ProductFormComponent', () => {
  let component: ProductFormComponent;
  let fixture: ComponentFixture<ProductFormComponent>;
  let productService: jasmine.SpyObj<ProductService>;
  let categoryService: jasmine.SpyObj<CategoryService>;

  const mockCategory: CategoryLookupDto = {
    id: 'cat-1',
    name: 'Categoría Test'
  };

  beforeEach(async () => {
    const productServiceSpy = jasmine.createSpyObj('ProductService', ['create', 'update']);
    const categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['search']);

    await TestBed.configureTestingModule({
      imports: [ProductFormComponent, ReactiveFormsModule, HttpClientTestingModule],
      providers: [
        { provide: ProductService, useValue: productServiceSpy },
        { provide: CategoryService, useValue: categoryServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ProductFormComponent);
    component = fixture.componentInstance;
    productService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    categoryService = TestBed.inject(CategoryService) as jasmine.SpyObj<CategoryService>;

    fixture.detectChanges();
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('debe actualizar valores del formulario al inicializar', () => {
      component.product = {
        id: '1',
        name: 'Producto Test',
        price: 100,
        stock: 10,
        categoryId: 'cat-1'
      } as ProductResponseDto;
      component.isEditing = true;
      component.visible = true;

      component.ngOnInit();

      expect(component.productForm.value.name).toBe('Producto Test');
    });
  });

  describe('ngOnChanges', () => {
    it('debe actualizar formulario cuando el producto cambia', () => {
      component.product = {
        id: '1',
        name: 'Producto Nuevo',
        price: 200
      } as ProductResponseDto;
      component.isEditing = true;
      component.visible = true;

      component.ngOnChanges({
        product: { currentValue: component.product, previousValue: null, firstChange: true, isFirstChange: () => true }
      });

      expect(component.productForm.value.name).toBe('Producto Nuevo');
    });

    it('debe resetear formulario cuando visible se vuelve false', () => {
      component.visible = true;
      component.productForm.patchValue({ name: 'Test' });

      component.ngOnChanges({
        visible: { currentValue: false, previousValue: true, firstChange: false, isFirstChange: () => false }
      });

      expect(component.productForm.value.name).toBe('');
    });
  });

  describe('onSubmit', () => {
    it('debe marcar formulario como touched si es inválido', () => {
      component.productForm.patchValue({ name: '' });

      component.onSubmit();

      expect(component.productForm.touched).toBeTrue();
      expect(productService.create).not.toHaveBeenCalled();
    });

    it('debe crear producto cuando el formulario es válido y no está editando', () => {
      component.isEditing = false;
      component.productForm.patchValue({
        name: 'Nuevo Producto',
        price: 100,
        stock: 10,
        categoryId: 'cat-1'
      });

      productService.create.and.returnValue(of({} as any));
      spyOn(component.saved, 'emit');
      spyOn(component, 'onCancel');

      component.onSubmit();

      expect(productService.create).toHaveBeenCalled();
      expect(component.saved.emit).toHaveBeenCalled();
    });

    it('debe actualizar producto cuando el formulario es válido y está editando', () => {
      component.isEditing = true;
      component.product = { id: '1' } as ProductResponseDto;
      component.productForm.patchValue({
        name: 'Producto Actualizado',
        price: 150,
        stock: 20,
        categoryId: 'cat-1'
      });

      productService.update.and.returnValue(of({} as any));
      spyOn(component.saved, 'emit');

      component.onSubmit();

      expect(productService.update).toHaveBeenCalledWith('1', jasmine.any(Object));
      expect(component.saved.emit).toHaveBeenCalled();
    });

    it('debe manejar error al crear', () => {
      component.isEditing = false;
      component.productForm.patchValue({
        name: 'Producto',
        price: 100,
        stock: 10,
        categoryId: 'cat-1'
      });

      const error = { error: { message: 'Error al crear' } };
      productService.create.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(component.apiError).toBe('Error al crear');
    });

    it('debe manejar error al actualizar', () => {
      component.isEditing = true;
      component.product = { id: '1' } as ProductResponseDto;
      component.productForm.patchValue({
        name: 'Producto',
        price: 100,
        stock: 10,
        categoryId: 'cat-1'
      });

      const error = { error: { message: 'Error al actualizar' } };
      productService.update.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(component.apiError).toBe('Error al actualizar');
    });

    it('debe recortar valores del formulario', () => {
      component.isEditing = false;
      component.productForm.patchValue({
        name: '  Producto  ',
        description: '  Descripción  ',
        price: 100,
        stock: 10,
        categoryId: 'cat-1'
      });

      productService.create.and.returnValue(of({} as any));

      component.onSubmit();

      expect(productService.create).toHaveBeenCalledWith(jasmine.objectContaining({
        name: 'Producto',
        description: 'Descripción'
      }));
    });
  });

  describe('onCancel', () => {
    it('debe resetear formulario y emitir cancel', () => {
      component.productForm.patchValue({ name: 'Test' });
      component.apiError = 'Error';
      spyOn(component.cancel, 'emit');

      component.onCancel();

      expect(component.productForm.value.name).toBe('');
      expect(component.apiError).toBe('');
      expect(component.cancel.emit).toHaveBeenCalled();
    });
  });

  describe('onCategorySearch', () => {
    it('debe buscar categorías después del debounce', fakeAsync(() => {
      categoryService.search.and.returnValue(of({ result: [mockCategory] } as any));

      component.onCategorySearch('test');

      expect(component.searchingCategories).toBeTrue();
      expect(categoryService.search).not.toHaveBeenCalled();

      tick(300);

      expect(categoryService.search).toHaveBeenCalledWith('test');
      expect(component.categorySearchResults).toEqual([mockCategory]);
      expect(component.searchingCategories).toBeFalse();
    }));

    it('debe limpiar resultados cuando el término de búsqueda está vacío', () => {
      component.categorySearchResults = [mockCategory];

      component.onCategorySearch('');

      expect(component.categorySearchResults).toEqual([]);
      expect(component.showCategoryDropdown).toBeFalse();
      expect(categoryService.search).not.toHaveBeenCalled();
    });

    it('debe manejar error de búsqueda', fakeAsync(() => {
      categoryService.search.and.returnValue(throwError(() => new Error('Error')));

      component.onCategorySearch('test');
      tick(300);

      expect(component.categorySearchResults).toEqual([]);
      expect(component.searchingCategories).toBeFalse();
      expect(component.showCategoryDropdown).toBeFalse();
    }));
  });

  describe('onCategorySelect', () => {
    it('debe ocultar dropdown de categorías', () => {
      component.showCategoryDropdown = true;

      component.onCategorySelect();

      expect(component.showCategoryDropdown).toBeFalse();
    });
  });

  describe('ngOnDestroy', () => {
    it('debe limpiar timeout de debounce', fakeAsync(() => {
      categoryService.search.and.returnValue(of({ result: [] } as any));

      component.onCategorySearch('test');
      component.ngOnDestroy();
      tick(300);

      expect(categoryService.search).not.toHaveBeenCalled();
    }));
  });
});


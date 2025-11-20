import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { CategoriesComponent } from './categories.component';
import { CategoryService } from '../../services/category.service';
import { CategoryResponseDto } from '../../swagger/model/categoryResponseDto';
import { CategoryResponseDtoPagedResult } from '../../swagger/model/categoryResponseDtoPagedResult';

describe('CategoriesComponent', () => {
  let component: CategoriesComponent;
  let fixture: ComponentFixture<CategoriesComponent>;
  let categoryService: jasmine.SpyObj<CategoryService>;

  const mockCategories: CategoryResponseDto[] = [
    { id: '1', name: 'Categoría 1', description: 'Descripción 1' } as CategoryResponseDto,
    { id: '2', name: 'Categoría 2', description: 'Descripción 2' } as CategoryResponseDto
  ];

  const mockPagedResult: CategoryResponseDtoPagedResult = {
    items: mockCategories,
    page: 1,
    pageSize: 10,
    totalCount: 2,
    totalPages: 1,
    hasPreviousPage: false,
    hasNextPage: false
  };

  beforeEach(async () => {
    const categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['getAll', 'delete']);

    await TestBed.configureTestingModule({
      imports: [CategoriesComponent, HttpClientTestingModule],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoriesComponent);
    component = fixture.componentInstance;
    categoryService = TestBed.inject(CategoryService) as jasmine.SpyObj<CategoryService>;
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('debe cargar categorías al inicializar', () => {
      categoryService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.ngOnInit();

      expect(categoryService.getAll).toHaveBeenCalledWith(1, 10, undefined);
      expect(component.categories).toEqual(mockCategories);
    });
  });

  describe('loadCategories', () => {
    it('debe cargar categorías exitosamente', () => {
      categoryService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.loadCategories();

      expect(component.categories).toEqual(mockCategories);
      expect(component.pagedResult).toEqual(mockPagedResult);
      expect(component.error).toBe('');
    });

    it('debe manejar error al cargar categorías', () => {
      categoryService.getAll.and.returnValue(throwError(() => new Error('Error')));

      component.loadCategories();

      expect(component.error).toBe('Error al cargar las categorías');
    });

    it('debe incluir searchTerm cuando se proporciona', () => {
      component.searchTerm = 'test';
      categoryService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.loadCategories();

      expect(categoryService.getAll).toHaveBeenCalledWith(1, 10, 'test');
    });
  });

  describe('search', () => {
    it('debe reiniciar página y cargar categorías', () => {
      component.currentPage = 3;
      component.searchTerm = 'test';
      categoryService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.search();

      expect(component.currentPage).toBe(1);
      expect(categoryService.getAll).toHaveBeenCalledWith(1, 10, 'test');
    });
  });

  describe('goToPage', () => {
    it('debe navegar a una página válida', () => {
      component.pagedResult = mockPagedResult;
      categoryService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.goToPage(1);

      expect(component.currentPage).toBe(1);
      expect(categoryService.getAll).toHaveBeenCalled();
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
      expect(component.selectedCategory).toBeNull();
      expect(component.showForm).toBeTrue();
      expect(component.error).toBe('');
    });
  });

  describe('openEditForm', () => {
    it('debe abrir formulario para editar', () => {
      const category = mockCategories[0];

      component.openEditForm(category);

      expect(component.isEditing).toBeTrue();
      expect(component.selectedCategory).toEqual(category);
      expect(component.showForm).toBeTrue();
      expect(component.error).toBe('');
    });
  });

  describe('closeForm', () => {
    it('debe cerrar formulario y resetear estado', () => {
      component.showForm = true;
      component.isEditing = true;
      component.selectedCategory = mockCategories[0];
      component.error = 'Error';

      component.closeForm();

      expect(component.showForm).toBeFalse();
      expect(component.isEditing).toBeFalse();
      expect(component.selectedCategory).toBeNull();
      expect(component.error).toBe('');
    });
  });

  describe('onCategorySaved', () => {
    it('debe recargar categorías y cerrar formulario', () => {
      component.showForm = true;
      categoryService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.onCategorySaved();

      expect(categoryService.getAll).toHaveBeenCalled();
      expect(component.showForm).toBeFalse();
    });
  });

  describe('openDeleteDialog', () => {
    it('debe abrir diálogo de eliminación con categoría', () => {
      const category = mockCategories[0];

      component.openDeleteDialog(category);

      expect(component.categoryToDelete).toEqual(category);
      expect(component.showDeleteDialog).toBeTrue();
    });
  });

  describe('onConfirmDelete', () => {
    it('debe eliminar categoría exitosamente', () => {
      component.categoryToDelete = mockCategories[0];
      categoryService.delete.and.returnValue(of({} as any));
      categoryService.getAll.and.returnValue(of({ result: mockPagedResult } as any));

      component.onConfirmDelete();

      expect(categoryService.delete).toHaveBeenCalledWith('1');
      expect(categoryService.getAll).toHaveBeenCalled();
      expect(component.showDeleteDialog).toBeFalse();
      expect(component.categoryToDelete).toBeNull();
    });

    it('debe manejar error al eliminar', () => {
      component.categoryToDelete = mockCategories[0];
      categoryService.delete.and.returnValue(throwError(() => ({ error: { message: 'Error al eliminar' } })));

      component.onConfirmDelete();

      expect(component.error).toBe('Error al eliminar');
      expect(component.showDeleteDialog).toBeFalse();
    });

    it('no debe eliminar si categoryToDelete es null', () => {
      component.categoryToDelete = null;

      component.onConfirmDelete();

      expect(categoryService.delete).not.toHaveBeenCalled();
    });
  });

  describe('onCancelDelete', () => {
    it('debe cerrar diálogo de eliminación', () => {
      component.showDeleteDialog = true;
      component.categoryToDelete = mockCategories[0];

      component.onCancelDelete();

      expect(component.showDeleteDialog).toBeFalse();
      expect(component.categoryToDelete).toBeNull();
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
    it('debe retornar mensaje con nombre de la categoría', () => {
      component.categoryToDelete = mockCategories[0];
      expect(component.deleteConfirmMessage).toContain('Categoría 1');
    });

    it('debe retornar mensaje genérico cuando la categoría no tiene nombre', () => {
      component.categoryToDelete = { id: '1' } as CategoryResponseDto;
      expect(component.deleteConfirmMessage).toContain('esta categoría');
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


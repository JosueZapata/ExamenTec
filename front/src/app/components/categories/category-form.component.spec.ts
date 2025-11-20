import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { CategoryFormComponent } from './category-form.component';
import { CategoryService } from '../../services/category.service';
import { CategoryResponseDto } from '../../swagger/model/categoryResponseDto';

describe('CategoryFormComponent', () => {
  let component: CategoryFormComponent;
  let fixture: ComponentFixture<CategoryFormComponent>;
  let categoryService: jasmine.SpyObj<CategoryService>;

  beforeEach(async () => {
    const categoryServiceSpy = jasmine.createSpyObj('CategoryService', ['create', 'update']);

    await TestBed.configureTestingModule({
      imports: [CategoryFormComponent, ReactiveFormsModule],
      providers: [
        { provide: CategoryService, useValue: categoryServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CategoryFormComponent);
    component = fixture.componentInstance;
    categoryService = TestBed.inject(CategoryService) as jasmine.SpyObj<CategoryService>;

    fixture.detectChanges();
  });

  it('debe crear el componente', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('debe actualizar valores del formulario al inicializar', () => {
      component.category = {
        id: '1',
        name: 'Categoría Test',
        description: 'Descripción Test'
      } as CategoryResponseDto;
      component.isEditing = true;
      component.visible = true;

      component.ngOnInit();

      expect(component.categoryForm.value.name).toBe('Categoría Test');
    });
  });

  describe('ngOnChanges', () => {
    it('debe actualizar formulario cuando la categoría cambia', () => {
      component.category = {
        id: '1',
        name: 'Categoría Nueva',
        description: 'Nueva descripción'
      } as CategoryResponseDto;
      component.isEditing = true;
      component.visible = true;

      component.ngOnChanges({
        category: { currentValue: component.category, previousValue: null, firstChange: true, isFirstChange: () => true }
      });

      expect(component.categoryForm.value.name).toBe('Categoría Nueva');
    });

    it('debe resetear formulario cuando visible se vuelve false', () => {
      component.visible = true;
      component.categoryForm.patchValue({ name: 'Test' });

      component.ngOnChanges({
        visible: { currentValue: false, previousValue: true, firstChange: false, isFirstChange: () => false }
      });

      expect(component.categoryForm.value.name).toBe('');
    });
  });

  describe('onSubmit', () => {
    it('debe marcar formulario como touched si es inválido', () => {
      component.categoryForm.patchValue({ name: '' });

      component.onSubmit();

      expect(component.categoryForm.touched).toBeTrue();
      expect(categoryService.create).not.toHaveBeenCalled();
    });

    it('debe crear categoría cuando el formulario es válido y no está editando', () => {
      component.isEditing = false;
      component.categoryForm.patchValue({
        name: 'Nueva Categoría',
        description: 'Descripción'
      });

      categoryService.create.and.returnValue(of({} as any));
      spyOn(component.saved, 'emit');
      spyOn(component, 'onCancel');

      component.onSubmit();

      expect(categoryService.create).toHaveBeenCalled();
      expect(component.saved.emit).toHaveBeenCalled();
    });

    it('debe actualizar categoría cuando el formulario es válido y está editando', () => {
      component.isEditing = true;
      component.category = { id: '1' } as CategoryResponseDto;
      component.categoryForm.patchValue({
        name: 'Categoría Actualizada',
        description: 'Nueva descripción'
      });

      categoryService.update.and.returnValue(of({} as any));
      spyOn(component.saved, 'emit');

      component.onSubmit();

      expect(categoryService.update).toHaveBeenCalledWith('1', jasmine.any(Object));
      expect(component.saved.emit).toHaveBeenCalled();
    });

    it('debe manejar error al crear', () => {
      component.isEditing = false;
      component.categoryForm.patchValue({
        name: 'Categoría',
        description: 'Descripción'
      });

      const error = { error: { message: 'Error al crear' } };
      categoryService.create.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(component.apiError).toBe('Error al crear');
    });

    it('debe manejar error al actualizar', () => {
      component.isEditing = true;
      component.category = { id: '1' } as CategoryResponseDto;
      component.categoryForm.patchValue({
        name: 'Categoría',
        description: 'Descripción'
      });

      const error = { error: { message: 'Error al actualizar' } };
      categoryService.update.and.returnValue(throwError(() => error));

      component.onSubmit();

      expect(component.apiError).toBe('Error al actualizar');
    });

    it('debe recortar valores del formulario', () => {
      component.isEditing = false;
      component.categoryForm.patchValue({
        name: '  Categoría  ',
        description: '  Descripción  '
      });

      categoryService.create.and.returnValue(of({} as any));

      component.onSubmit();

      expect(categoryService.create).toHaveBeenCalledWith(jasmine.objectContaining({
        name: 'Categoría',
        description: 'Descripción'
      }));
    });

    it('debe establecer description a null si está vacío después de recortar', () => {
      component.isEditing = false;
      component.categoryForm.patchValue({
        name: 'Categoría',
        description: '   '
      });

      categoryService.create.and.returnValue(of({} as any));

      component.onSubmit();

      expect(categoryService.create).toHaveBeenCalledWith(jasmine.objectContaining({
        description: null
      }));
    });
  });

  describe('onCancel', () => {
    it('debe resetear formulario y emitir cancel', () => {
      component.categoryForm.patchValue({ name: 'Test' });
      component.apiError = 'Error';
      spyOn(component.cancel, 'emit');

      component.onCancel();

      expect(component.categoryForm.value.name).toBe('');
      expect(component.apiError).toBe('');
      expect(component.cancel.emit).toHaveBeenCalled();
    });
  });

  describe('onAlertClosed', () => {
    it('debe limpiar apiError', () => {
      component.apiError = 'Error';

      component.onAlertClosed();

      expect(component.apiError).toBe('');
    });
  });
});


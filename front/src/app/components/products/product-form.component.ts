import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductRequestDto } from '../../swagger/model/productRequestDto';
import { ProductResponseDto } from '../../swagger/model/productResponseDto';
import { CategoryResponseDto } from '../../swagger/model/categoryResponseDto';
import { CategoryLookupDto } from '../../swagger/model/categoryLookupDto';
import { AlertComponent } from '../shared/alert/alert.component';
import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AlertComponent],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})
export class ProductFormComponent implements OnInit, OnChanges, OnDestroy {
  @Input() visible: boolean = false;
  @Input() product: ProductResponseDto | null = null;
  @Input() isEditing: boolean = false;
  @Output() saved = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  apiError: string = '';
  categories: CategoryResponseDto[] = [];
  searchingCategories: boolean = false;
  categorySearchTerm: string = '';
  categorySearchResults: CategoryLookupDto[] = [];
  showCategoryDropdown: boolean = false;

  productForm!: FormGroup;

  private categorySearchDebounce?: ReturnType<typeof setTimeout>;

  constructor(
    private readonly fb: FormBuilder,
    private readonly productService: ProductService,
    private readonly categoryService: CategoryService
  ) {
    this.initializeForm();
  }

  get categoryOptions() {
    if (this.categorySearchTerm.trim()) {
      return this.categorySearchResults;
    }
    return this.categories;
  }

  get categoryDropdownSize(): number {
    return Math.min(this.categoryOptions.length + 1, 6);
  }

  ngOnInit(): void {
    this.updateFormValues();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['product'] || changes['isEditing'] || changes['visible']) {
      this.updateFormValues();
    }
    
    if (changes['visible'] && !changes['visible'].currentValue) {
      this.resetForm();
    }
  }

  ngOnDestroy(): void {
    if (this.categorySearchDebounce) {
      clearTimeout(this.categorySearchDebounce);
    }
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.apiError = '';
    const formValue = this.productForm.value;
    const request: ProductRequestDto = {
      name: formValue.name?.trim() || '',
      description: formValue.description?.trim() || null,
      price: formValue.price ?? 0,
      stock: formValue.stock ?? 0,
      categoryId: formValue.categoryId ?? null
    };

    const operation = this.isEditing && this.product?.id
      ? this.productService.update(this.product.id, request)
      : this.productService.create(request);

    operation.subscribe({
      next: () => {
        this.saved.emit();
        this.onCancel();
      },
      error: (err: HttpErrorResponse) => {
        this.apiError = err.error?.message || 
          (this.isEditing ? 'Error al actualizar el producto' : 'Error al crear el producto');
        console.error(err);
      }
    });
  }

  onCancel(): void {
    this.resetForm();
    this.apiError = '';
    this.cancel.emit();
  }

  onAlertClosed(): void {
    this.apiError = '';
  }

  onCategorySearch(term: string): void {
    this.categorySearchTerm = term;

    if (this.categorySearchDebounce) {
      clearTimeout(this.categorySearchDebounce);
    }

    if (!term.trim()) {
      this.categorySearchResults = [];
      this.searchingCategories = false;
      this.showCategoryDropdown = false;
      return;
    }

    this.searchingCategories = true;
    this.categorySearchDebounce = setTimeout(() => {
      this.categoryService.search(term).subscribe({
        next: (response) => {
          this.categorySearchResults = response.result ?? [];
          this.searchingCategories = false;
          this.showCategoryDropdown = this.categorySearchResults.length > 0;
        },
        error: () => {
          this.categorySearchResults = [];
          this.searchingCategories = false;
          this.showCategoryDropdown = false;
        }
      });
    }, 300);
  }

  onCategorySelect(): void {
    this.showCategoryDropdown = false;
  }

  private initializeForm(): void {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      price: [null, [Validators.required, Validators.min(0)]],
      stock: [null, [Validators.required, Validators.min(0)]],
      categoryId: ['', [Validators.required]]
    });
  }

  private updateFormValues(): void {
    if (!this.productForm) {
      return;
    }

    if (this.product && this.isEditing) {
      this.patchFormValues();
    } else if (!this.visible) {
      this.resetForm();
    }
  }

  private patchFormValues(): void {
    this.productForm.patchValue({
      name: this.product?.name || '',
      description: this.product?.description || '',
      price: this.product?.price ?? null,
      stock: this.product?.stock ?? null,
      categoryId: this.product?.categoryId || ''
    }, { emitEvent: false });

    if (this.product?.categoryId) {
      this.categories = [{
        id: this.product.categoryId,
        name: this.product.categoryName || 'Categoría actual'
      } as CategoryResponseDto];
    }
  }

  private resetForm(): void {
    this.productForm.reset({
      name: '',
      description: '',
      price: null,
      stock: null,
      categoryId: ''
    }, { emitEvent: false });
    this.productForm.markAsUntouched();
    this.productForm.markAsPristine();

    this.categorySearchTerm = '';
    this.categorySearchResults = [];
    this.categories = [];
    this.showCategoryDropdown = false;
  }

}


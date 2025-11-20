import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { CategoryRequestDto } from '../../swagger/model/categoryRequestDto';
import { CategoryResponseDto } from '../../swagger/model/categoryResponseDto';
import { AlertComponent } from '../shared/alert/alert.component';
import { CategoryService } from '../../services/category.service';

interface FormConfig {
  name: any[];
  description: any[];
}

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AlertComponent],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss'
})
export class CategoryFormComponent implements OnInit, OnChanges {
  @Input() visible: boolean = false;
  @Input() category: CategoryResponseDto | null = null;
  @Input() isEditing: boolean = false;
  @Output() saved = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  apiError: string = '';

  categoryForm!: FormGroup;

  private readonly formConfig: FormConfig = {
    name: [Validators.required, Validators.maxLength(200)],
    description: [Validators.maxLength(500)]
  };

  constructor(
    private readonly fb: FormBuilder,
    private readonly categoryService: CategoryService
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.updateFormValues();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['category'] || changes['isEditing'] || changes['visible']) {
      this.updateFormValues();
    }
    
    if (changes['visible'] && !changes['visible'].currentValue) {
      this.resetForm();
    }
  }


  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    this.apiError = '';
    const formValue = this.categoryForm.value;
    const request: CategoryRequestDto = {
      name: formValue.name?.trim() || '',
      description: formValue.description?.trim() || null
    };

    const operation = this.isEditing && this.category?.id
      ? this.categoryService.update(this.category.id, request)
      : this.categoryService.create(request);

    operation.subscribe({
      next: () => {
        this.saved.emit();
        this.onCancel();
      },
      error: (err) => {
        this.apiError = err.error?.message || 
          (this.isEditing ? 'Error al actualizar la categoría' : 'Error al crear la categoría');
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

  private initializeForm(): void {
    this.categoryForm = this.fb.group({
      name: ['', this.formConfig.name],
      description: ['', this.formConfig.description]
    });
  }

  private updateFormValues(): void {
    if (!this.categoryForm) {
      return;
    }

    if (this.category && this.isEditing) {
      this.patchFormValues();
    } else if (!this.visible) {
      this.resetForm();
    }
  }

  private patchFormValues(): void {
    this.categoryForm.patchValue({
      name: this.category?.name || '',
      description: this.category?.description || ''
    }, { emitEvent: false });
  }

  private resetForm(): void {
    this.categoryForm.reset({
      name: '',
      description: ''
    }, { emitEvent: false });
    this.categoryForm.markAsUntouched();
    this.categoryForm.markAsPristine();
  }
}


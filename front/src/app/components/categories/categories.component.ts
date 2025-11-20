import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../services/category.service';
import { CategoryRequestDto } from '../../swagger/model/categoryRequestDto';
import { CategoryResponseDto } from '../../swagger/model/categoryResponseDto';
import { CategoryResponseDtoPagedResult } from '../../swagger/model/categoryResponseDtoPagedResult';
import { CategoryFormComponent } from './category-form.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { AlertComponent } from '../shared/alert/alert.component';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, CategoryFormComponent, ConfirmDialogComponent, AlertComponent],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  categories: CategoryResponseDto[] = [];
  pagedResult: CategoryResponseDtoPagedResult | null = null;
  currentPage: number = 1;
  pageSize: number = 10;
  searchTerm: string = '';
  
  showForm: boolean = false;
  isEditing: boolean = false;
  selectedCategory: CategoryResponseDto | null = null;
  
  showDeleteDialog: boolean = false;
  categoryToDelete: CategoryResponseDto | null = null;
  
  error: string = '';

  constructor(private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.error = '';
    
    this.categoryService.getAll(this.currentPage, this.pageSize, this.searchTerm || undefined)
      .subscribe({
        next: (response) => {
          if (response.result) {
            this.pagedResult = response.result;
            this.categories = response.result.items || [];
          }
        },
        error: (err) => {
          this.error = 'Error al cargar las categorías';
          console.error(err);
        }
      });
  }

  search(): void {
    this.currentPage = 1;
    this.loadCategories();
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.currentPage = 1;
    this.loadCategories();
  }

  goToPage(page: number): void {
    if (page >= 1 && this.pagedResult && page <= (this.pagedResult.totalPages || 1)) {
      this.currentPage = page;
      this.loadCategories();
    }
  }

  openCreateForm(): void {
    this.isEditing = false;
    this.selectedCategory = null;
    this.showForm = true;
    this.error = '';
  }

  openEditForm(category: CategoryResponseDto): void {
    this.isEditing = true;
    this.selectedCategory = category;
    this.showForm = true;
    this.error = '';
  }

  closeForm(): void {
    this.showForm = false;
    this.isEditing = false;
    this.selectedCategory = null;
    this.error = '';
  }

  onCategorySaved(): void {
    this.loadCategories();
    this.closeForm();
  }

  openDeleteDialog(category: CategoryResponseDto): void {
    this.categoryToDelete = category;
    this.showDeleteDialog = true;
  }

  onConfirmDelete(): void {
    if (!this.categoryToDelete?.id) return;
    
    this.error = '';
    this.showDeleteDialog = false;
    
    this.categoryService.delete(this.categoryToDelete.id)
      .subscribe({
        next: () => {
          this.loadCategories();
        },
        error: (err) => {
          this.error = err.error?.message || 'Error al eliminar la categoría';
          console.error(err);
        }
      });
    
    this.categoryToDelete = null;
  }

  onCancelDelete(): void {
    this.showDeleteDialog = false;
    this.categoryToDelete = null;
  }

  get totalPages(): number {
    return this.pagedResult?.totalPages || 1;
  }

  get hasPreviousPage(): boolean {
    return this.pagedResult?.hasPreviousPage || false;
  }

  get hasNextPage(): boolean {
    return this.pagedResult?.hasNextPage || false;
  }

  get deleteConfirmMessage(): string {
    if (!this.categoryToDelete?.name) {
      return '¿Estás seguro de eliminar esta categoría? Esta acción no se puede deshacer.';
    }
    return `¿Estás seguro de eliminar la categoría "${this.categoryToDelete.name}"? Esta acción no se puede deshacer.`;
  }

  onAlertClosed(): void {
    this.error = '';
  }
}

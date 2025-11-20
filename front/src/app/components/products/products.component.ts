import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { ProductRequestDto } from '../../swagger/model/productRequestDto';
import { ProductResponseDto } from '../../swagger/model/productResponseDto';
import { ProductResponseDtoPagedResult } from '../../swagger/model/productResponseDtoPagedResult';
import { ProductFormComponent } from './product-form.component';
import { ConfirmDialogComponent } from '../shared/confirm-dialog/confirm-dialog.component';
import { AlertComponent } from '../shared/alert/alert.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, ProductFormComponent, ConfirmDialogComponent, AlertComponent],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  products: ProductResponseDto[] = [];
  pagedResult: ProductResponseDtoPagedResult | null = null;
  currentPage: number = 1;
  pageSize: number = 10;
  searchTerm: string = '';
  
  showForm: boolean = false;
  isEditing: boolean = false;
  selectedProduct: ProductResponseDto | null = null;
  
  showDeleteDialog: boolean = false;
  productToDelete: ProductResponseDto | null = null;
  
  error: string = '';

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.error = '';
    
    this.productService.getAll(this.currentPage, this.pageSize, this.searchTerm || undefined)
      .subscribe({
        next: (response) => {
          if (response.result) {
            this.pagedResult = response.result;
            this.products = response.result.items || [];
          }
        },
        error: (err) => {
          this.error = 'Error al cargar los productos';
          console.error(err);
        }
      });
  }

  search(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  goToPage(page: number): void {
    if (page >= 1 && this.pagedResult && page <= (this.pagedResult.totalPages || 1)) {
      this.currentPage = page;
      this.loadProducts();
    }
  }

  openCreateForm(): void {
    this.isEditing = false;
    this.selectedProduct = null;
    this.showForm = true;
    this.error = '';
  }

  openEditForm(product: ProductResponseDto): void {
    this.isEditing = true;
    this.selectedProduct = product;
    this.showForm = true;
    this.error = '';
  }

  closeForm(): void {
    this.showForm = false;
    this.isEditing = false;
    this.selectedProduct = null;
    this.error = '';
  }

  onProductSaved(): void {
    this.loadProducts();
    this.closeForm();
  }

  openDeleteDialog(product: ProductResponseDto): void {
    this.productToDelete = product;
    this.showDeleteDialog = true;
  }

  onConfirmDelete(): void {
    if (!this.productToDelete?.id) return;
    
    this.error = '';
    this.showDeleteDialog = false;
    
    this.productService.delete(this.productToDelete.id)
      .subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (err) => {
          this.error = err.error?.message || 'Error al eliminar el producto';
          console.error(err);
        }
      });
    
    this.productToDelete = null;
  }

  onCancelDelete(): void {
    this.showDeleteDialog = false;
    this.productToDelete = null;
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
    if (!this.productToDelete?.name) {
      return '¿Estás seguro de eliminar este producto? Esta acción no se puede deshacer.';
    }
    return `¿Estás seguro de eliminar el producto "${this.productToDelete.name}"? Esta acción no se puede deshacer.`;
  }

  formatPrice(price: number | undefined): string {
    if (price === undefined || price === null) return '-';
    return new Intl.NumberFormat('es-MX', { style: 'currency', currency: 'MXN' }).format(price);
  }

  onAlertClosed(): void {
    this.error = '';
  }
}


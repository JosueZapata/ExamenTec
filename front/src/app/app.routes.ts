import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/inicio', pathMatch: 'full' },
  {
    path: 'iniciar-sesion',
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'acceso-denegado',
    loadComponent: () => import('./components/forbidden/forbidden.component').then(m => m.ForbiddenComponent)
  },
  {
    path: 'inicio',
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard]
  },
  {
    path: 'categorias',
    loadComponent: () => import('./components/categories/categories.component').then(m => m.CategoriesComponent),
    canActivate: [authGuard]
  },
  {
    path: 'productos',
    loadComponent: () => import('./components/products/products.component').then(m => m.ProductsComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/inicio' }
];

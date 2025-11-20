import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { LoginResponseDto } from '../../../swagger/model/loginResponseDto';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit, OnDestroy {
  currentUser: LoginResponseDto | null = null;
  private userSubscription?: Subscription;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userSubscription = this.authService.currentUser$.subscribe((user: LoginResponseDto | null) => {
      this.currentUser = user;
    });
  }

  ngOnDestroy(): void {
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }

  logout(): void {
    this.authService.logout();
  }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  getUserRole(): string | null {
    return this.currentUser?.role || null;
  }

  canAccessCategories(): boolean {
    const role = this.getUserRole();
    return role === 'Admin' || role === 'Category';
  }

  canAccessProducts(): boolean {
    const role = this.getUserRole();
    return role === 'Admin' || role === 'Product';
  }

  isCategoriesDisabled(): boolean {
    return !this.canAccessCategories();
  }

  isProductsDisabled(): boolean {
    return !this.canAccessProducts();
  }
}


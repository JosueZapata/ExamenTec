import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { AlertComponent } from '../shared/alert/alert.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, AlertComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  email: string = '';
  password: string = '';
  error: string = '';
  loading: boolean = false;
  returnUrl: string = '/inicio';
  showPassword: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    if (this.authService.isAuthenticated()) {
      this.router.navigate([this.returnUrl]);
      return;
    }

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/inicio';
  }

  onSubmit(): void {
    if (!this.email || !this.password) {
      this.error = 'Por favor ingrese email y contraseña';
      return;
    }

    this.error = '';
    this.loading = true;

    this.authService.login(this.email, this.password).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.result) {
          this.router.navigate([this.returnUrl]);
        }
      },
      error: (err) => {
        this.loading = false;
        if (err.error?.message) {
          this.error = err.error.message;
        } else if (err.error?.errors) {
          const errors = Object.values(err.error.errors).flat();
          this.error = Array.isArray(errors) ? errors.join(', ') : 'Error al iniciar sesión';
        } else {
          this.error = 'Email o contraseña incorrectos';
        }
      }
    });
  }

  onAlertClosed(): void {
    this.error = '';
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  copyCredentials(email: string, password: string): void {
    this.email = email;
    this.password = password;

    if (navigator?.clipboard?.writeText) {
      navigator.clipboard.writeText(`${email} / ${password}`).catch(() => { });
    }
  }
}


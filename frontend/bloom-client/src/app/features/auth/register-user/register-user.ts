import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register-user',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register-user.html',
  styleUrl: './register-user.css'
})
export class RegisterUser {
  errorMessage = '';
  isSubmitting = false;

  registerForm;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.errorMessage = '';
    this.isSubmitting = true;

    const request = {
      firstName: this.registerForm.value.firstName ?? '',
      lastName: this.registerForm.value.lastName ?? '',
      email: this.registerForm.value.email ?? '',
      password: this.registerForm.value.password ?? ''
    };

    this.authService.register(request).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['/products']);
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = error.error?.message ?? 'Registration failed. Please try again.';
      }
    });
  }
}
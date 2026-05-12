import { AsyncPipe, CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { OrderService } from '../../core/services/order.service';
import { AuthService } from '../../core/services/auth.service';
import { CheckoutRequest, OrderResponse } from '../../shared/models/checkout.model';

@Component({
  selector: 'app-checkout',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, AsyncPipe],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class Checkout {
  cartItems$;
  cartTotal$;

  isSubmitting = false;
  errorMessage = '';

  checkoutForm;

  constructor(
    private formBuilder: FormBuilder,
    private cartService: CartService,
    private orderService: OrderService,
    private authService: AuthService,
    private router: Router
  ) {
    this.cartItems$ = this.cartService.cartItems$;
    this.cartTotal$ = this.cartService.cartTotal$;

    this.checkoutForm = this.formBuilder.group({
      fullName: ['', [Validators.required, Validators.maxLength(150)]],
      streetAddress: ['', [Validators.required, Validators.maxLength(200)]],
      city: ['', [Validators.required, Validators.maxLength(100)]],
      county: ['', [Validators.required, Validators.maxLength(100)]],
      postalCode: ['', [Validators.required, Validators.maxLength(20)]],
      country: ['Romania', [Validators.required, Validators.maxLength(100)]],
      phoneNumber: ['', [Validators.required, Validators.maxLength(30)]]
    });
  }

  placeOrder(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }

    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    const cartItems = this.cartService.getCurrentItems();

    if (cartItems.length === 0) {
      this.errorMessage = 'Your cart is empty.';
      return;
    }

    const formValue = this.checkoutForm.value;

    const shippingAddress = [
      `Name: ${formValue.fullName}`,
      `Street: ${formValue.streetAddress}`,
      `City: ${formValue.city}`,
      `County/State: ${formValue.county}`,
      `Postal Code: ${formValue.postalCode}`,
      `Country: ${formValue.country}`,
      `Phone: ${formValue.phoneNumber}`
    ].join(', ');

    const request: CheckoutRequest = {
      shippingAddress,
      items: cartItems.map(item => ({
        productId: item.product.id,
        quantity: item.quantity
      }))
    };

    this.isSubmitting = true;
    this.errorMessage = '';

    this.orderService.checkout(request).subscribe({
      next: (response: OrderResponse) => {
        this.isSubmitting = false;
        this.cartService.clearCart();

        this.router.navigate(['/order-success'], {
          state: {
            order: response
          }
        });
      },
      error: (error) => {
        this.isSubmitting = false;

        if (error.status === 401) {
          this.errorMessage = 'Please log in before placing your order.';
          return;
        }

        this.errorMessage =
          error.error?.message ?? 'Could not place the order. Please try again.';
      }
    });
  }
}
import { AsyncPipe, CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { CartItemComponent } from '../../shared/components/cart-item-component/cart-item-component';

@Component({
  selector: 'app-cart',
  imports: [CommonModule, AsyncPipe, RouterLink, CartItemComponent],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class Cart {
  cartItems$;
  cartTotal$;

  constructor(private cartService: CartService) {
    this.cartItems$ = this.cartService.cartItems$;
    this.cartTotal$ = this.cartService.cartTotal$;
  }

  increaseQuantity(productId: number): void {
    this.cartService.increaseQuantity(productId);
  }

  decreaseQuantity(productId: number): void {
    this.cartService.decreaseQuantity(productId);
  }

  removeFromCart(productId: number): void {
    this.cartService.removeFromCart(productId);
  }

  clearCart(): void {
    this.cartService.clearCart();
  }
}

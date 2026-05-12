import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { Product } from '../../shared/models/product.model';
import { CartItem } from '../../shared/models/cart-item.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly cartItemsSubject = new BehaviorSubject<CartItem[]>([]);

  cartItems$ = this.cartItemsSubject.asObservable();

  cartCount$ = this.cartItems$.pipe(
    map(items => items.reduce((total, item) => total + item.quantity, 0))
  );

  cartTotal$ = this.cartItems$.pipe(
    map(items =>
      items.reduce((total, item) => total + item.product.price * item.quantity, 0)
    )
  );

  addToCart(product: Product): void {
    const currentItems = this.cartItemsSubject.value;

    const existingItem = currentItems.find(
      item => item.product.id === product.id
    );

    if (existingItem) {
      const updatedItems = currentItems.map(item =>
        item.product.id === product.id
          ? { ...item, quantity: item.quantity + 1 }
          : item
      );

      this.cartItemsSubject.next(updatedItems);
      return;
    }

    const newItem: CartItem = {
      product,
      quantity: 1
    };

    this.cartItemsSubject.next([...currentItems, newItem]);
  }

  increaseQuantity(productId: number): void {
    const updatedItems = this.cartItemsSubject.value.map(item =>
      item.product.id === productId
        ? { ...item, quantity: item.quantity + 1 }
        : item
    );

    this.cartItemsSubject.next(updatedItems);
  }

  decreaseQuantity(productId: number): void {
    const updatedItems = this.cartItemsSubject.value
      .map(item =>
        item.product.id === productId
          ? { ...item, quantity: item.quantity - 1 }
          : item
      )
      .filter(item => item.quantity > 0);

    this.cartItemsSubject.next(updatedItems);
  }

  removeFromCart(productId: number): void {
    const updatedItems = this.cartItemsSubject.value.filter(
      item => item.product.id !== productId
    );

    this.cartItemsSubject.next(updatedItems);
  }

  clearCart(): void {
    this.cartItemsSubject.next([]);
  }

  getCurrentItems(): CartItem[] {
    return this.cartItemsSubject.value;
  }
}
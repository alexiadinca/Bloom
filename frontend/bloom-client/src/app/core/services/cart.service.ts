import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { Product } from '../../shared/models/product.model';
import { CartItem } from '../../shared/models/cart-item.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly cartStorageKey = 'bloom_cart';
  private readonly cartItemsSubject = new BehaviorSubject<CartItem[]>(
    this.getStoredCartItems()
  );

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
    if (product.stockQuantity <= 0) {
      return;
    }

    const currentItems = this.cartItemsSubject.value;

    const existingItem = currentItems.find(
      item => item.product.id === product.id
    );

    if (existingItem) {
      if (existingItem.quantity >= product.stockQuantity) {
        return;
      }

      const updatedItems = currentItems.map(item =>
        item.product.id === product.id
          ? { ...item, quantity: item.quantity + 1 }
          : item
      );

      this.updateCart(updatedItems);
      return;
    }

    const newItem: CartItem = {
      product,
      quantity: 1
    };

    this.updateCart([...currentItems, newItem]);
  }

  increaseQuantity(productId: number): void {
    const updatedItems = this.cartItemsSubject.value.map(item =>
      item.product.id === productId && item.quantity < item.product.stockQuantity
        ? { ...item, quantity: item.quantity + 1 }
        : item
    );

    this.updateCart(updatedItems);
  }

  decreaseQuantity(productId: number): void {
    const updatedItems = this.cartItemsSubject.value
      .map(item =>
        item.product.id === productId
          ? { ...item, quantity: item.quantity - 1 }
          : item
      )
      .filter(item => item.quantity > 0);

    this.updateCart(updatedItems);
  }

  removeFromCart(productId: number): void {
    const updatedItems = this.cartItemsSubject.value.filter(
      item => item.product.id !== productId
    );

    this.updateCart(updatedItems);
  }

  clearCart(): void {
    this.updateCart([]);
  }

  getCurrentItems(): CartItem[] {
    return this.cartItemsSubject.value;
  }

  private updateCart(items: CartItem[]): void {
    localStorage.setItem(this.cartStorageKey, JSON.stringify(items));
    this.cartItemsSubject.next(items);
  }

  private getStoredCartItems(): CartItem[] {
    const storedCartItems = localStorage.getItem(this.cartStorageKey);

    if (!storedCartItems) {
      return [];
    }

    try {
      return JSON.parse(storedCartItems) as CartItem[];
    } catch {
      localStorage.removeItem(this.cartStorageKey);
      return [];
    }
  }
}

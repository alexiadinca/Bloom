import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CartItem } from '../../models/cart-item.model';

@Component({
  selector: 'app-cart-item-component',
  imports: [CommonModule],
  templateUrl: './cart-item-component.html',
  styleUrl: './cart-item-component.css'
})
export class CartItemComponent {
  @Input({ required: true }) item!: CartItem;
  @Output() increase = new EventEmitter<number>();
  @Output() decrease = new EventEmitter<number>();
  @Output() remove = new EventEmitter<number>();

  get reachedStockLimit(): boolean {
    return this.item.quantity >= this.item.product.stockQuantity;
  }

  increaseQuantity(): void {
    if (this.reachedStockLimit) {
      return;
    }

    this.increase.emit(this.item.product.id);
  }

  decreaseQuantity(): void {
    this.decrease.emit(this.item.product.id);
  }

  removeFromCart(): void {
    this.remove.emit(this.item.product.id);
  }
}

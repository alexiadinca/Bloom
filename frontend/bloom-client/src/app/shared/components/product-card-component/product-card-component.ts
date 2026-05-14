import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-product-card-component',
  imports: [CommonModule],
  templateUrl: './product-card-component.html',
  styleUrl: './product-card-component.css'
})
export class ProductCardComponent {
  @Input({ required: true }) product!: Product;
  @Output() addProduct = new EventEmitter<Product>();

  addToCart(): void {
    this.addProduct.emit(this.product);
  }
}

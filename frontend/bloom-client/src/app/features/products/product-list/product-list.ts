import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../shared/models/product.model';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css'
})
export class ProductList implements OnInit {
  products: Product[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load products. Please try again later.';
        this.isLoading = false;
      }
    });
  }
}
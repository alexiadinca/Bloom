import { CommonModule, Location } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OrderResponse } from '../../shared/models/checkout.model';

@Component({
  selector: 'app-order-success',
  imports: [CommonModule, RouterLink],
  templateUrl: './order-success.html',
  styleUrl: './order-success.css'
})
export class OrderSuccess {
  order: OrderResponse | null = null;

  constructor(private location: Location) {
    const state = this.location.getState() as { order?: OrderResponse };
    this.order = state.order ?? null;
  }
}
import { AsyncPipe, NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, AsyncPipe, NgIf],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar {
  cartCount$;
  currentUser$;

  constructor(
    private cartService: CartService,
    private authService: AuthService,
    private router: Router
  ) {
    this.cartCount$ = this.cartService.cartCount$;
    this.currentUser$ = this.authService.currentUser$;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/products']);
  }
}
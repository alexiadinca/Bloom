import { Routes } from '@angular/router';
import { ProductList } from './features/products/product-list/product-list';
import { Cart } from './features/cart/cart';
import { Login } from './features/auth/login/login';
import { RegisterUser } from './features/auth/register-user/register-user';
import { Checkout } from './features/checkout/checkout';
import { OrderSuccess } from './features/order-success/order-success';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full'
  },
  {
    path: 'products',
    component: ProductList
  },
  {
    path: 'cart',
    component: Cart
  },
  {
    path: 'checkout',
    component: Checkout
  },
  {
    path: 'order-success',
    component: OrderSuccess
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'register',
    component: RegisterUser
  }
];
export interface CheckoutItemRequest {
  productId: number;
  quantity: number;
}

export interface CheckoutRequest {
  shippingAddress: string;
  items: CheckoutItemRequest[];
}

export interface OrderItemResponse {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface OrderResponse {
  orderId: number;
  totalPrice: number;
  shippingAddress: string;
  createdAt: string;
  items: OrderItemResponse[];
}
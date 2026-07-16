export type User = {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  birthDate: string;
  createdAt: string;
  role: string;
};

export type UserAuth = {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
};

export type Product = {
  id: number;
  category: Category | null;
  sku: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  imageUrl: string | null;
};

export type Category = {
  id: number;
  slug: string;
  name: string;
};

export type Country = {
  name: string;
  cca2: string;
};

export type Address = {
  id: number;
  clientId: number;
  addressLine1: string;
  addressLine2: string;
  country: string;
  countryCode: string;
  region: string;
  city: string;
  postalCode: string;
};

export type CartItem = {
  product: Product;
  quantity: number;
};

export type Cart = {
  id: number;
  clientId: number;
  items: CartItem[];
  totalPrice: number;
  totalProducts: number;
};

export type OrderItem = {
  productId: number;
  productName: string;
  productSku: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
};

export type OrderStatus = 'PENDING' | 'PAID' | 'EXPIRED'

export type Order = {
  id: number;
  clientId: number | null;
  clientEmail: string | null;
  clientName: string | null;
  address: Address;
  orderDate: string;
  items: OrderItem[];
  status: OrderStatus;
  totalPrice: number;
  totalProducts: number;
};

export type CheckoutSession = {
  sessionId: string;
  url: string;
  amount: number;
  currency: string;
  expiresAt: string;
  order: Order;
};

type ErrorType =
  | 'email_already_used'
  | 'phone_already_used'
  | 'invalid_country_code'
  | 'password_update'
  | 'insufficient_stock';

export type Api422ErrorBody = {
  errorType: ErrorType;
  message: string;
  details: unknown;
};

export type PaginatedResponse<T> = {
  limit: number;
  page: number;
  totalPages: number;
  items: T[]
}
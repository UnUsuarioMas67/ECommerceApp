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
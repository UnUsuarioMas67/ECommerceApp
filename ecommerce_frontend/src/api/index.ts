import type { AxiosInstance } from 'axios';
import type { Category, Product, User } from './types';

export const apiUrl = 'http://localhost:5113/api';
export const imagesUrl = 'http://localhost:5113/images';

// TODO: Handle get current user request in the AuthProvider
export async function getCurrentUser(axiosInstace: AxiosInstance) {
  const response = await axiosInstace.get<User>('/clients/me', {
    validateStatus: (status) => status === 200 || status === 401,
  });

  if (response.status === 401) return null;

  return response.data;
}

export async function fetchCategories(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<Category[]>('/categories');
  return response.data;
}

type FetchProductOptions = {
  searchTerm?: string;
  category?: string;
};

export async function fetchProducts(axiosInstance: AxiosInstance, options?: FetchProductOptions): Promise<Product[]> {
  const { searchTerm, category } = options ?? {};
  
  let url = '/products';
  if (category) url += `/categories/${category}`;
  if (searchTerm) url += `?search=${searchTerm}`;

  const response = await axiosInstance.get<Product[]>(url);
  return response.data;
}

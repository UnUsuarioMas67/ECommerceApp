import type { AxiosInstance } from 'axios';
import type { Category, Product, User, UserAuth } from './types';
import type { LoginRequest, RegisterRequest } from '../schemas';

export const apiUrl = 'http://localhost:5113/api';
export const imagesUrl = 'http://localhost:5113/images';

export async function postLogin(axiosInstance: AxiosInstance, data: LoginRequest) {
  const response = await axiosInstance.post<UserAuth>('/clients/login', data);
  return response.data;
}

export async function postLogout(axiosInstance: AxiosInstance) {
  await axiosInstance.post('clients/logout');
}

export async function postRegister(axiosInstance: AxiosInstance, data: RegisterRequest) {
  const response = await axiosInstance.post<User>('/clients/login', data);
  return response.data;
}

export async function fetchCategories(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<Category[]>('/categories');
  return response.data;
}

export async function fetchCategory(axiosInstance: AxiosInstance, category: number | string) {
  const response = await axiosInstance.get<Category>(`/categories/${category}`);
  return response.data;
}

type FetchProductOptions = {
  searchTerm?: string;
  category?: string;
  itemsPerPage?: number;
  pageParam: number;
};

export async function fetchProducts(axiosInstance: AxiosInstance, options: FetchProductOptions) {
  const { searchTerm, category, pageParam, itemsPerPage } = options;

  let url = '/products';
  if (category) url += `/categories/${category}`;

  url += `?page=${pageParam}`;
  if (searchTerm) url += `&search=${searchTerm}`;
  url += `&limit=${itemsPerPage ?? 16}`;

  const data = (await axiosInstance.get<Product[]>(url)).data;

  return {
    data,
    currentPage: pageParam,
    nextPage: data.length > 0 ? pageParam + 1 : null,
  };
}

export async function fetchProduct(axiosInstance: AxiosInstance, id: number) {
  const response = await axiosInstance.get<Product>(`/products/${id}`);
  return response.data;
}

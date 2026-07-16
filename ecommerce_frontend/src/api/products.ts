import type { AxiosInstance } from "axios";
import type { PaginatedResponse, Product } from "./types";

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

  const response = await axiosInstance.get<PaginatedResponse<Product>>(url);

  return response.data;
}

export async function fetchProduct(axiosInstance: AxiosInstance, id: number) {
  const response = await axiosInstance.get<Product>(`/products/${id}`);
  return response.data;
}
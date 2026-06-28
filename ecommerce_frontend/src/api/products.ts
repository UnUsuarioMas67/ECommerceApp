import type { AxiosInstance } from "axios";
import type { Product } from "./types";

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
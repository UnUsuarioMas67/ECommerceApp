import type { AxiosInstance } from "axios";
import type { Category } from "./types";

export async function fetchCategories(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<Category[]>('/categories');
  return response.data;
}

export async function fetchCategory(axiosInstance: AxiosInstance, category: number | string) {
  const response = await axiosInstance.get<Category>(`/categories/${category}`);
  return response.data;
}
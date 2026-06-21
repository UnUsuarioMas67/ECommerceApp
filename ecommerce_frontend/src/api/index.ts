import type { AxiosInstance } from "axios";
import type { Category, User } from "../types/api-types";

export const apiUrl = 'http://localhost:5113/api';
export const imagesUrl = 'http://localhost:5113/images';

export async function getCurrentUser(axiosInstace: AxiosInstance) {
  const response = await axiosInstace.get<User>('/clients/me', {
    validateStatus: (status) => status === 200 || status === 401,
  });

  if (response.status === 401)
    return null;

  return response.data;
}

export async function getCategories(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<Category[]>('/categories')
  return response.data;
}
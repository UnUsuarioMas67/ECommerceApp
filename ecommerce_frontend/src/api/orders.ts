import type { AxiosInstance } from 'axios';
import type { Order } from './types';

export async function fetchOrders(axiosInstance: AxiosInstance) {
  const response = await axiosInstance.get<Order[]>('/orders/me');
  return response.data;
}

export async function fetchOrder(axiosInstance: AxiosInstance, id: number) {
  const response = await axiosInstance.get<Order>(`/orders/me/${id}`, {
    validateStatus: (status) => status === 200 || status === 404,
  });

  if (response.status === 404) return null;

  return response.data;
}

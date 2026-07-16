import type { AxiosInstance } from 'axios';
import type { Order, PaginatedResponse } from './types';

type FetchOrdersOptions = {
  itemsPerPage?: number;
  pageParam: number;
};

export async function fetchOrders(axiosInstance: AxiosInstance, options: FetchOrdersOptions) {
  const { pageParam, itemsPerPage } = options;
  
    let url = '/orders/me';
    url += `?page=${pageParam}`;
    url += `&limit=${itemsPerPage ?? 16}`;
  
    const response = await axiosInstance.get<PaginatedResponse<Order>>(url);
  
    return response.data;
}

export async function fetchOrder(axiosInstance: AxiosInstance, id: number) {
  const response = await axiosInstance.get<Order>(`/orders/me/${id}`, {
    validateStatus: (status) => status === 200 || status === 404,
  });

  if (response.status === 404) return null;

  return response.data;
}

import type { AxiosInstance } from 'axios';
import type { CheckoutRequest } from '../schemas/checkout';

export async function createCheckoutSession(axiosInstance: AxiosInstance, data: CheckoutRequest) {
  const response = await axiosInstance.post('/checkout/session', { ...data, successUrl: 'http://localhost:5173', cancelUrl: '' });
  return response.data;
}

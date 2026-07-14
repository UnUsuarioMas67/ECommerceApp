import { isAxiosError, type AxiosInstance } from 'axios';
import type { CheckoutRequest } from '../schemas/checkout';
import type { Api422ErrorBody } from './types';

export async function createCheckoutSession(axiosInstance: AxiosInstance, data: CheckoutRequest) {
  try {
    const response = await axiosInstance.post('/checkout/session', data);
    return response.data;
  } catch (error) {
    if (isAxiosError(error)) {

      if (error.response?.status === 422) {
        const errorObj = error.response.data as Api422ErrorBody;

        if (errorObj.errorType === 'insufficient_stock') {
          const details = errorObj.details as ProductsStockErrorDetails;
          throw new ProductsStockError(details.products)
        }
      }
    }

    throw error;
  }
}

export function isProductsStockError(obj: unknown) {
  return obj instanceof ProductsStockError
}

export type ProductsStockErrorItem = {
  productId: number;
  quantityRequested: number;
  stockAvailable: number;
};

type ProductsStockErrorDetails = {
  products: ProductsStockErrorItem[];
};

export class ProductsStockError extends Error implements ProductsStockErrorDetails {
  products: ProductsStockErrorItem[];

  constructor(products: ProductsStockErrorItem[]) {
    super('One or more items have insufficient stock');
    this.name = 'ProductsStockError';
    this.products = products;
  }
}

import { createContext, useContext } from 'react';
import type { ProductsStockErrorItem } from '../../api/checkout';

type CheckoutErrorContextType = {
  error: string | null;
  setError: (value: string | null) => void;
  products: ProductsStockErrorItem[];
  setProducts: (value: ProductsStockErrorItem[]) => void;
};

export const CheckoutErrorContext = createContext<CheckoutErrorContextType | null>(null);

export function useCheckoutError() {
  const context = useContext(CheckoutErrorContext);
  if (!context) throw new Error('useCheckoutError must be used within an CheckoutErrorProvider');

  return context;
}

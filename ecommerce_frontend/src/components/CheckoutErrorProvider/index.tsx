import React, { useState } from 'react';
import { CheckoutErrorContext } from './CheckoutErrorContext';
import type { ProductsStockErrorItem } from '../../api/checkout';

type Props = {
  children: React.ReactNode;
};

function CheckoutErrorProvider({ children }: Props) {
  const [error, setError] = useState<string | null>(null);
  const [products, setProducts] = useState<ProductsStockErrorItem[]>([]);

  return <CheckoutErrorContext value={{ error, setError, products, setProducts }}>{children}</CheckoutErrorContext>;
}

export default CheckoutErrorProvider;

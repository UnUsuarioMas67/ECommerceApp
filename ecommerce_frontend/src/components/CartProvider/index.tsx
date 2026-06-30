import React, { useCallback, useState } from 'react';
import { CartContext } from './CartContext';
import type { CartItem } from '../../api/types';

type Props = {
  children: React.ReactNode;
};

// TODO: Save cart data to local storage
// TODO: Sync with backend cart
function CartProvider({ children }: Props) {
  const [items, setItems] = useState<CartItem[]>([]);

  const validItem = useCallback(({ product, quantity }: CartItem) => {
    let validQty = quantity;

    if (validQty <= 0) {
      validQty = 0;
    } else if (validQty > product.stock) {
      validQty = product.stock;
    }

    return { product, quantity: validQty };
  }, []);

  const getItem = useCallback((id: number) => items.find((item) => item.product.id === id), [items]);

  const removeItem = useCallback((id: number) => {
    setItems((prev) => prev.filter((ci) => ci.product.id !== id));
  }, []);

  const setItem = useCallback(
    (item: CartItem) => {
      const {
        product: { id },
        quantity,
      } = item;

      if (getItem(id)) {
        if (quantity > 0)
          setItems((prev) =>
            prev.map((i) => {
              if (id === i.product.id) return validItem(item);
              else return i;
            }),
          );
        else removeItem(id);
      } else if (quantity > 0) {
        setItems((prev) => [...prev, validItem(item)]);
      }
    },
    [getItem, validItem, removeItem],
  );

  const clearCart = useCallback(() => {
    setItems([]);
  }, []);

  return <CartContext value={{ items, setItem, removeItem, clearCart, getItem }}>{children}</CartContext>;
}

export default CartProvider;

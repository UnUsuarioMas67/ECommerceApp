import { createContext, useContext } from 'react';
import type { CartItem } from '../../api/types';

type CartContextType = {
  items: CartItem[];
  setItem: (item: CartItem) => void;
  removeItem: (id: number) => void;
  getItem: (id: number) => CartItem | undefined;
  clearCart: () => void;
};

export const CartContext = createContext<CartContextType | null>(null);

export function useCart() {
  const context = useContext(CartContext);
  if (!context) throw new Error('useCart must be used within an CartProvider');

  return context;
}

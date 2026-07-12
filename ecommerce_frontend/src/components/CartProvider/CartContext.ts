import { createContext, useContext } from 'react';
import type { CartItemRequest } from '../../schemas/checkout';

type CartContextType = {
  items: CartItemRequest[];
  getItemQuantity: (productId: number) => number;
  increaseItemQuantity: (productId: number) => void;
  decreaseItemQuantity: (productId: number) => void;
  removeItem: (productId: number) => void;
  clearCart: () => void;
  openCart: () => void;
  closeCart: () => void;
  isCartOpen: boolean;
  totalItems: number;
};

export const CartContext = createContext<CartContextType | null>(null);

export function useCart() {
  const context = useContext(CartContext);
  if (!context) throw new Error('useCart must be used within an CartProvider');

  return context;
}

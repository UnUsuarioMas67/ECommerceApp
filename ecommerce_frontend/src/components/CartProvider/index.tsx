import { CartContext, type CartItem } from './CartContext';
import { useLocalStorage } from '../../hooks/use-localstorage';
import { useState } from 'react';

type Props = {
  children: React.ReactNode;
};

function CartProvider({ children }: Props) {
  const [cartItems, setCartItems] = useLocalStorage<CartItem[]>('shoppingCart', []);
  const [isCartOpen, setIsCartOpen] = useState(false);

  const getItemQuantity = (productId: number) => {
    return cartItems.find((item) => item.productId === productId)?.quantity || 0;
  };

  const increaseItemQuantity = (productId: number) => {
    setCartItems((prev) => {
      if (!prev.find((item) => item.productId === productId)) {
        return [...prev, { productId, quantity: 1 }];
      } else {
        return prev.map((item) => {
          if (item.productId === productId) {
            return { ...item, quantity: item.quantity + 1 };
          } else {
            return item;
          }
        });
      }
    });
  };

  const decreaseItemQuantity = (productId: number) => {
    setCartItems((prev) => {
      if (prev.find((item) => item.productId === productId)?.quantity === 1) {
        return prev.filter((item) => item.productId !== productId);
      } else {
        return prev.map((item) => {
          if (item.productId === productId) {
            return { ...item, quantity: item.quantity - 1 };
          } else {
            return item;
          }
        });
      }
    });
  };

  const removeItem = (productId: number) => {
    setCartItems((prev) => prev.filter((item) => item.productId !== productId));
  };

  const clearCart = () => {
    setCartItems(() => []);
  };

  const totalItems = cartItems.map((item) => item.quantity).reduce((prev, curr) => prev + curr, 0);

  return (
    <CartContext
      value={{
        items: cartItems,
        getItemQuantity,
        increaseItemQuantity,
        decreaseItemQuantity,
        removeItem,
        clearCart,
        isCartOpen,
        openCart: () => setIsCartOpen(true),
        closeCart: () => setIsCartOpen(false),
        totalItems,
      }}>
      {children}
    </CartContext>
  );
}

export default CartProvider;

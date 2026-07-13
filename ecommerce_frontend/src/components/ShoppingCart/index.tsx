import Offcanvas from 'react-bootstrap/esm/Offcanvas';
import { useCart } from '../CartProvider/CartContext';
import ShoppingCartItem from './ShoppingCartItem';
import Button from 'react-bootstrap/esm/Button';
import { queryOptions, useQueries } from '@tanstack/react-query';
import { useAxios } from '../../hooks/use-axios';
import { fetchProduct } from '../../api/products';
import LoadingSpinner from '../LoadingSpinner';
import { Trash } from 'react-bootstrap-icons';
import { useEffect } from 'react';
import { useNavigate } from '@tanstack/react-router';

function ShoppingCart() {
  const { isCartOpen, closeCart, items, totalItems, getItemQuantity, clearCart } = useCart();
  const axiosInstance = useAxios();
  const navigate = useNavigate();
  const { products, isPending, hasErrors } = useQueries({
    queries: items.map((item) =>
      queryOptions({
        queryKey: ['products', item.productId],
        queryFn: () => fetchProduct(axiosInstance, item.productId),
        staleTime: 1000 * 60 * 5, // 5 mins
        refetchOnWindowFocus: false,
      }),
    ),
    combine: (result) => {
      return {
        products: result.map((query) => query.data).filter((product) => !!product),
        isPending: result.some((query) => query.isPending),
        hasErrors: result.some((query) => query.isError),
      };
    },
  });

  useEffect(() => {
    if (items.length === 0) closeCart();
  }, [items, closeCart]);

  const totalCost = products
    .map((product) => product.price * getItemQuantity(product.id))
    .reduce((prev, curr) => prev + curr, 0)
    .toFixed(2);

  const onPayBtnClick = () => {
    closeCart();
    navigate({ to: '/checkout' });
  };

  return (
    <Offcanvas show={isCartOpen} onHide={closeCart} placement="end" scroll>
      <Offcanvas.Header closeButton>
        <Offcanvas.Title>Shopping cart ({totalItems})</Offcanvas.Title>
      </Offcanvas.Header>
      <Offcanvas.Body>
        {isPending ? (
          <LoadingSpinner />
        ) : products.length === 0 ? (
          <span className="text-body-tertiary">No items in the cart.</span>
        ) : hasErrors ? (
          <>
            <span className="text-danger">Could not load the items in cart</span>
            <br />
            <Button variant="link" onClick={clearCart}>
              <Trash /> Clear cart
            </Button>
          </>
        ) : (
          <>
            <div className="mb-2">
              {products.map((product) => (
                <ShoppingCartItem product={product} key={product.id} />
              ))}
            </div>
            <div className="mb-4">
              {hasErrors && <span className="text-danger">One or more items in cart could not be loaded</span>}
              <Button variant="link" onClick={clearCart}>
                <Trash /> Clear cart
              </Button>
            </div>

            <div className="px-4">
              <Button className="w-100" variant="success" onClick={onPayBtnClick}>
                Pay: <strong>${totalCost}</strong>
              </Button>
            </div>
          </>
        )}
      </Offcanvas.Body>
    </Offcanvas>
  );
}

export default ShoppingCart;

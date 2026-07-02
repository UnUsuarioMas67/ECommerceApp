import Offcanvas from 'react-bootstrap/esm/Offcanvas';
import { useCart } from '../CartProvider/CartContext';
import ShoppingCartItem from './ShoppingCartItem';
import Button from 'react-bootstrap/esm/Button';

function ShoppingCartSidebar() {
  const { isCartOpen, closeCart, items, totalItems } = useCart();

  return (
    <Offcanvas show={isCartOpen} onHide={closeCart} placement="end" scroll>
      <Offcanvas.Header closeButton>
        <Offcanvas.Title>Shopping cart ({totalItems})</Offcanvas.Title>
      </Offcanvas.Header>
      <Offcanvas.Body>
        {items.length === 0 ? (
          <span className="text-body-tertiary">No items in the cart.</span>
        ) : (
          <>
            <div className="mb-5">
              {items.map((item) => (
                <ShoppingCartItem item={item} key={item.productId} />
              ))}
            </div>
            <div className='px-4'>
              <Button className="w-100" variant="success">
                Pay 
              </Button>
            </div>
          </>
        )}
      </Offcanvas.Body>
    </Offcanvas>
  );
}

export default ShoppingCartSidebar;

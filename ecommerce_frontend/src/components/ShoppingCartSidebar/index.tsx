import Offcanvas from 'react-bootstrap/esm/Offcanvas';
import { useCart } from '../CartProvider/CartContext';


function ShoppingCartSidebar() {
  const { isCartOpen, closeCart } = useCart()
  
  return (
    <Offcanvas show={isCartOpen} onHide={closeCart} placement="end" scroll>
      <Offcanvas.Header closeButton>
        <Offcanvas.Title>Shopping cart</Offcanvas.Title>
      </Offcanvas.Header>
      <Offcanvas.Body>
        <span className="text-body-tertiary">No items yet.</span>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

export default ShoppingCartSidebar;

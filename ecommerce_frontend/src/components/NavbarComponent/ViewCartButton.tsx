import { Cart4 } from 'react-bootstrap-icons';
import Nav from 'react-bootstrap/esm/Nav';
import Badge from 'react-bootstrap/esm/Badge';
import { useCart } from '../CartProvider/CartContext';

function ViewCartButton() {
  const { openCart, totalItems } = useCart();

  return (
    <Nav.Link className="ms-2 position-relative" onClick={openCart} disabled={totalItems === 0}>
      <Cart4 size={24} title="View cart" />
      {totalItems > 0 && (
        <Badge className='position-absolute top-0 start-100 translate-middle' bg="dark" pill>
          {totalItems}
        </Badge>
      )}
      <span className="visually-hidden">items in cart</span>
    </Nav.Link>
  );
}

export default ViewCartButton;

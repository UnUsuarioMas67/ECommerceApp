import { Cart4 } from 'react-bootstrap-icons';
import Nav from 'react-bootstrap/esm/Nav';
import { useCart } from '../CartProvider/CartContext';

function ViewCartButton() {
  const {openCart, totalItems} = useCart()

  return (
    <Nav.Link className="ms-2" onClick={openCart} disabled={totalItems === 0}>
      <Cart4 size={24} title="View cart" /> {totalItems > 0 && <span className='small'>{totalItems}</span>}
    </Nav.Link>
  );
}

export default ViewCartButton;

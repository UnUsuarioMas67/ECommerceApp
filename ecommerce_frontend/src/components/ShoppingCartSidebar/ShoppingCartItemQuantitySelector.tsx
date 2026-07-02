import { DashLg, PlusLg } from 'react-bootstrap-icons';
import Button from 'react-bootstrap/esm/Button';
import InputGroup from 'react-bootstrap/esm/InputGroup';
import type { Product } from '../../api/types';
import { useCart } from '../CartProvider/CartContext';

type Props = {
  product: Product;
};

function ShoppingCartItemQuantitySelector({ product }: Props) {
  const { decreaseItemQuantity, increaseItemQuantity, getItemQuantity } = useCart();
  const quantity = getItemQuantity(product.id);

  return (
    <InputGroup size="sm">
      <Button variant="outline-primary" onClick={() => decreaseItemQuantity(product.id)} disabled={quantity === 1}>
        <DashLg />
      </Button>

      <InputGroup.Text id="quantity" className="justify-content-center border-primary" >
        {quantity}
      </InputGroup.Text>

      <Button variant="outline-primary" onClick={() => increaseItemQuantity(product.id)} disabled={product.stock <= quantity}>
        <PlusLg />
      </Button>
    </InputGroup>
  );
}

export default ShoppingCartItemQuantitySelector;

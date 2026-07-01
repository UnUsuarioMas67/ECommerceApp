import Button from 'react-bootstrap/Button';
import type { Product } from '../api/types';
import { Plus, Dash, Trash } from 'react-bootstrap-icons';
import InputGroup from 'react-bootstrap/InputGroup';
import { useCart } from './CartProvider/CartContext';

type Props = {
  product: Product;
  disabled?: boolean;
};

function AddToCartButton({ product, disabled }: Props) {
  const { getItemQuantity, increaseItemQuantity, decreaseItemQuantity } = useCart();

  const style = { maxWidth: '144px', width: '100%' };
  const quantity = getItemQuantity(product.id);

  return quantity <= 0 ? (
    <Button
      disabled={product.stock == 0 || disabled}
      variant="primary"
      onClick={() => increaseItemQuantity(product.id)}
      style={style}>
      {product.stock > 0 ? 'Add to cart' : 'Out of stock'}
    </Button>
  ) : (
    <>
      <InputGroup style={style}>
        <Button variant="primary" onClick={() => decreaseItemQuantity(product.id)}>
          {quantity > 1 ? <Dash /> : <Trash />}
        </Button>

        <InputGroup.Text id="basic-addon2" className='flex-sm-grow-1 justify-content-center'>{getItemQuantity(product.id)}</InputGroup.Text>

        <Button variant="primary" onClick={() => increaseItemQuantity(product.id)} disabled={product.stock <= quantity}>
          <Plus />
        </Button>
      </InputGroup>
    </>
  );
}

export default AddToCartButton;

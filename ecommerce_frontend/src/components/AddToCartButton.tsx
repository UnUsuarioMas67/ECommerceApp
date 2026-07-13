import Button from 'react-bootstrap/Button';
import type { Product } from '../api/types';
import { Plus, Dash, Trash, CartPlusFill } from 'react-bootstrap-icons';
import InputGroup from 'react-bootstrap/InputGroup';
import { useCart } from './CartProvider/CartContext';

type Props = {
  product: Product;
  disabled?: boolean;
  quantityOnly?: boolean
};

function AddToCartButton({ product, disabled, quantityOnly }: Props) {
  const { getItemQuantity, increaseItemQuantity, decreaseItemQuantity } = useCart();

  const style = { maxWidth: '144px', width: '100%', height: '40px' };
  const quantity = getItemQuantity(product.id);

  return quantity <= 0 && !quantityOnly ? (
    <Button
      disabled={product.stock == 0 || disabled}
      variant="primary"
      onClick={() => increaseItemQuantity(product.id)}
      style={style}>
      {product.stock > 0 ? (
        <span className="d-inline-flex align-items-center gap-2">
          <CartPlusFill /> Add to cart
        </span>
      ) : (
        'Out of stock'
      )}
    </Button>
  ) : (
    <>
      <InputGroup style={style}>
        <Button variant="primary" onClick={() => decreaseItemQuantity(product.id)} disabled={quantity === 1 && quantityOnly}>
          {quantity > 1 || quantityOnly ? <Dash /> : <Trash />}
        </Button>

        <InputGroup.Text id="basic-addon2" className="flex-sm-grow-1 justify-content-center">
          {getItemQuantity(product.id)}
        </InputGroup.Text>

        <Button variant="primary" onClick={() => increaseItemQuantity(product.id)} disabled={product.stock <= quantity}>
          <Plus />
        </Button>
      </InputGroup>
    </>
  );
}

export default AddToCartButton;

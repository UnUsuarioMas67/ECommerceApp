import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import type { Product } from '../api/types';
import { Plus, Dash, Trash } from 'react-bootstrap-icons';
import InputGroup from 'react-bootstrap/InputGroup';
import Form from 'react-bootstrap/Form';

type Props = {
  product: Product;
  disabled?: boolean;
};

function AddToCartButton({ product, disabled }: Props) {
  // TODO: Create a CartProvider to manage the shopping cart

  const [mode, setMode] = useState<'button' | 'counter'>('button');
  const [quantity, setQuantity] = useState(0);

  const style = { maxWidth: '144px', width: '144px' };

  const updateQuantity = (value: number) => {
    let n = value;

    if (n <= 0)
    {
      n = 0;
      setMode('button')
    } else if (n > product.stock) {
      n = product.stock
    }
    
    setQuantity(n)
  }

  const onBtnClick = () => {
    setQuantity(1);
    setMode('counter');
  };

  const onDeleteClick = () => {
    updateQuantity(0)
  }

  const onInputChange = (value: string) => {
    const n = value === '' ? 0 : parseInt(value);
    if (!isNaN(n)) updateQuantity(n);
  };

  return mode === 'button' ? (
    <Button disabled={product.stock == 0 || disabled} variant="primary" onClick={onBtnClick} style={style}>
      {product.stock > 0 ? 'Add to cart' : 'Out of stock'}
    </Button>
  ) : (
    <InputGroup style={style}>
      {quantity > 1 ? (
        <Button variant="primary" onClick={() => updateQuantity(quantity - 1)}>
          <Dash />
        </Button>
      ) : (
        <Button variant="primary" onClick={onDeleteClick}>
          <Trash />
        </Button>
      )}

      <Form.Control
        className="bg-secondary text-center"
        value={quantity}
        onChange={(e) => onInputChange(e.target.value)}
      />

      <Button variant="primary" onClick={() => updateQuantity(quantity + 1)} disabled={product.stock <= quantity}>
        <Plus />
      </Button>
    </InputGroup>
  );
}

export default AddToCartButton;

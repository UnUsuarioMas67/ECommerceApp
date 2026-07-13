import Card from 'react-bootstrap/Card';
import type { Product } from '../api/types';
import { Link } from '@tanstack/react-router';
import AddToCartButton from './AddToCartButton';
import ProductImage from './ProductImage';
import { useCart } from './CartProvider/CartContext';

type Props = {
  product: Product;
  subtotal?: boolean;
  disableLink?: boolean;
  quantityOnly?: boolean;
  noInput?: boolean;
};

function ProductCard({ product, subtotal, disableLink, quantityOnly, noInput }: Props) {
  const { getItemQuantity } = useCart();
  const quantityInCart = getItemQuantity(product.id);

  return (
    <Card>
      <div className="d-flex flex-column align-items-center flex-sm-row">
        <ProductImage
          productSrc={product.imageUrl ?? ''}
          fallback="https://placehold.co/180"
          className="rounded-start"
          style={{ height: '180px', width: '180px' }}
        />
        <Card.Body className="h-100">
          <Card.Title>
            {disableLink ? (
              product.name
            ) : (
              <Link to="/products/$productId" params={{ productId: product.id.toString() }} title={product.name}>
                {product.name}
              </Link>
            )}
          </Card.Title>

          <Card.Subtitle className="mb-2 fw-normal text-body-secondary">{product.category?.name}</Card.Subtitle>

          <Card.Text className="fs-4 fw-bold">
            $ {product.price}{' '}
            {subtotal && quantityInCart > 1 && `x${quantityInCart} = $ ${quantityInCart * product.price}`}
          </Card.Text>

          {!noInput && <AddToCartButton product={product} quantityOnly={quantityOnly} />}
        </Card.Body>
      </div>
    </Card>
  );
}

export default ProductCard;

import Card from 'react-bootstrap/Card';
import type { Product } from '../api/types';
import { Link } from '@tanstack/react-router';
import AddToCartButton from './AddToCartButton';
import ProductImage from './ProductImage';

function ProductCard({ product }: { product: Product }) {
  return (
    <Card>
      <div className="d-flex">
        <ProductImage
          productSrc={product.imageUrl ?? ''}
          fallback="https://placehold.co/180"
          className="rounded-start"
          style={{ height: '180px', width: '180px' }}
        />
        <Card.Body className="h-100">
          <Card.Title>
            <Link to="/products/$productId" params={{ productId: product.id.toString() }} title={product.name}>
              {product.name}
            </Link>
          </Card.Title>

          <Card.Subtitle className="mb-2 fw-normal text-body-secondary">{product.category?.name}</Card.Subtitle>

          <Card.Text className="fs-4 fw-bold">${product.price}</Card.Text>

          <AddToCartButton product={product} />
        </Card.Body>
      </div>
    </Card>
  );
}

export default ProductCard;

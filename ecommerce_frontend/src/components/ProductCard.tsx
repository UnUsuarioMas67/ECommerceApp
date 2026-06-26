import Card from 'react-bootstrap/Card';
import Image from 'react-bootstrap/Image';
import type { Product } from '../api/types';
import { Link } from '@tanstack/react-router';
import AddToCartButton from './AddToCartButton';

function ProductCard({ product, imagesUrl }: { product: Product; imagesUrl: string }) {
  const fallbackImageUrl = 'https://placehold.co/180';
  const imageSrc = product.imageUrl ? `${imagesUrl}/products/${product.imageUrl}` : fallbackImageUrl;

  return (
    <Card>
      <div className="d-flex">
        <Image
          src={imageSrc}
          onError={(e) => {
            e.currentTarget.onerror = null;
            e.currentTarget.src = fallbackImageUrl;
          }}
          className="rounded-start"
          style={{ height: '180px', width: '180px' }}
          fluid
        />
        <Card.Body className="h-100">
          <Card.Title>
            <Link to="/products/$productId" params={{ productId: product.id.toString() }} title={product.name}>
              {product.name}
            </Link>
          </Card.Title>

          <Card.Subtitle className="mb-2 fw-normal text-body-secondary">{product.category?.name}</Card.Subtitle>
          
          <Card.Text className="fs-4 fw-bold">${product.price}</Card.Text>

          <AddToCartButton product={product}/>
        </Card.Body>
      </div>
    </Card>
  );
}

export default ProductCard;

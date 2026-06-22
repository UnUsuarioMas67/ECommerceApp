import Card from 'react-bootstrap/Card';
import Button from 'react-bootstrap/Button';
import type { Product } from '../api/types';

function ProductCard({ product, imagesUrl }: { product: Product; imagesUrl: string }) {
  const fallbackImageUrl = 'https://placehold.co/180';
  const imageSrc = product.imageUrl ? `${imagesUrl}/products/${product.imageUrl}` : fallbackImageUrl;

  return (
    <Card>
      <Card.Img
        variant="top"
        src={imageSrc}
        onError={(e) => {
          e.currentTarget.onerror = null;
          e.currentTarget.src = fallbackImageUrl;
        }}
      />
      <Card.Header>{product.sku}</Card.Header>
      <Card.Body>
        <Card.Title>{product.name}</Card.Title>
        <Card.Subtitle className="mb-2 text-primary-emphasis">${product.price}</Card.Subtitle>
        <Card.Text>{product.description}</Card.Text>
        <Button disabled={product.stock == 0} variant="primary">
          {product.stock > 0 ? 'Add to cart' : 'Out of stock'}
        </Button>
      </Card.Body>
    </Card>
  );
}

export default ProductCard;

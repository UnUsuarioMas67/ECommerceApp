import { useQuery } from '@tanstack/react-query';
import { useCart, type CartItem } from '../CartProvider/CartContext';
import { fetchProduct } from '../../api/products';
import { useAxios } from '../../hooks/use-axios';
import LoadingSpinner from '../LoadingSpinner';
import Card from 'react-bootstrap/esm/Card';
import ProductImage from '../ProductImage';
import { Link } from '@tanstack/react-router';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import { X } from 'react-bootstrap-icons';
import Button from 'react-bootstrap/esm/Button';
import ShoppingCartItemQuantitySelector from './ShoppingCartItemQuantitySelector';

type Props = {
  item: CartItem;
};

function ShoppingCartItem({ item: { productId } }: Props) {
  const axiosInstance = useAxios();
  const { data: product, status } = useQuery({
    queryKey: ['products', productId],
    queryFn: () => fetchProduct(axiosInstance, productId),
    staleTime: 1000 * 60 * 5,
  });

  const { removeItem } = useCart();

  return status === 'pending' ? (
    <LoadingSpinner />
  ) : status === 'error' ? (
    <div>Error</div>
  ) : (
    <Card className="border-0 rounded-0 border-bottom">
      <Row className="g-2">
        <Col xs={3} className="d-flex align-items-center">
          <ProductImage productSrc={product.imageUrl ?? ''} fallback="https://placehold.co/180" fluid />
        </Col>
        <Col>
          <Card.Body className="px-0">
            <Card.Title className="fs-6">
              <Link to="/products/$productId" params={{ productId: product.id.toString() }} title={product.name}>
                {product.name}
              </Link>
            </Card.Title>

            <Card.Subtitle className="mb-2 small fw-normal text-body-secondary">{product.category?.name}</Card.Subtitle>

            <Card.Text className="fw-bold d-flex align-items-center gap-2" >
              ${product.price}
              <ShoppingCartItemQuantitySelector product={product} />
            </Card.Text>
          </Card.Body>
        </Col>
        <Col className="d-flex align-items-center justify-content-center" xs={2}>
          <Button variant="outline-secondary" className="border-0 p-1" onClick={() => removeItem(productId)}>
            <X size={24} />
          </Button>
        </Col>
      </Row>
    </Card>
  );
}

export default ShoppingCartItem;

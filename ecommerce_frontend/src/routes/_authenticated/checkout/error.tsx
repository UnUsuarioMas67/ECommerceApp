import { createFileRoute } from '@tanstack/react-router';
import { XCircleFill } from 'react-bootstrap-icons';
import Container from 'react-bootstrap/esm/Container';
import { useCheckoutError } from '../../../components/CheckoutErrorProvider/CheckoutErrorContext';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import ProductCard from '../../../components/ProductCard';
import { useAxios } from '../../../hooks/use-axios';
import type { AxiosInstance } from 'axios';
import { queryOptions, useSuspenseQueries } from '@tanstack/react-query';
import { fetchProduct } from '../../../api/products';

export const Route = createFileRoute('/_authenticated/checkout/error')({
  component: RouteComponent,
});

const productsQuery = (axiosInstance: AxiosInstance, productId: number) =>
  queryOptions({
    queryKey: ['products', productId],
    queryFn: () => fetchProduct(axiosInstance, productId),
    staleTime: 1000 * 60 * 5, // 5 mins
    refetchOnWindowFocus: false,
  });

function RouteComponent() {
  const { error, setError, products: invalidProducts, setProducts } = useCheckoutError();

  const axiosInstance = useAxios();
  const { products } = useSuspenseQueries({
    queries: invalidProducts.map((p) => productsQuery(axiosInstance, p.productId)),
    combine: (result) => {
      return {
        products: result.filter((query) => !query.isError).map((query) => query.data!),
        isPending: result.some((query) => query.isPending),
      };
    },
  });

  return (
    <Container as="main">
      <Row>
        <Col>
          <div className="text-center mb-3">
            <div className="text-danger mb-3">
              <XCircleFill size={172} />
            </div>
            <h3 className="text-body">{error}</h3>
          </div>

          {invalidProducts.length > 0 && (
            <div>
              <p className="text-center">One or more products have recently ran out of stock</p>
              <Row xs={1}>
                {products.map((product) => (
                  <Col>
                    <ProductCard product={product} subtotal noInput disableLink />
                    <p className="text-body-secondary">
                      {invalidProducts.find((p) => p.productId === product.id)!.stockAvailable > 0
                        ? 'Not enough stock'
                        : 'Out of stock'}
                    </p>
                  </Col>
                ))}
              </Row>
            </div>
          )}
        </Col>
      </Row>
    </Container>
  );
}

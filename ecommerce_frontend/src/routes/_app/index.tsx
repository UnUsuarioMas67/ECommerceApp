import { createFileRoute } from '@tanstack/react-router';
import { useAxios } from '../../hooks/use-axios';
import { useQuery } from '@tanstack/react-query';
import type { Product } from '../../types/api-types';
import type { AxiosInstance } from 'axios';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import ProductCard from '../../components/ProductCard';

export const Route = createFileRoute('/_app/')({
  component: RouteComponent,
});

async function fetchProducts(axiosInstance: AxiosInstance): Promise<Product[]> {
  const response = await axiosInstance.get<Product[]>('/products');
  return response.data;
}

function RouteComponent() {
  const axiosInstance = useAxios();
  const {
    data: products,
    isLoading,
    error,
  } = useQuery({ queryKey: ['products'], queryFn: () => fetchProducts(axiosInstance), staleTime: 5 * 1000 });

  if (isLoading) return <p>Loading...</p>;
  if (error) return <p>Error: {error.message}</p>;

  return (
    <>
      <title>{products ? `${products.length} Products` : 'Home'}</title>

      <h1>Products</h1>
      <Row xs={1} md={2} lg={3} xl={4} className="g-3">
        {products &&
          products.map((product) => (
            <Col key={product.id}>
              <ProductCard product={product} apiBaseUrl="http://localhost:5113" />
            </Col>
          ))}
      </Row>
    </>
  );
}

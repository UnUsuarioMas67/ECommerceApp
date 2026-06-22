import { createFileRoute } from '@tanstack/react-router';
import { useAxios } from '../../hooks/use-axios';
import { useQuery } from '@tanstack/react-query';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import ProductCard from '../../components/ProductCard';
import { fetchProducts, imagesUrl } from '../../api';
import z from 'zod';

const searchSchema = z.object({
  searchTerm: z.string().optional(),
  category: z.string().optional(),
});

export const Route = createFileRoute('/_app/')({
  component: RouteComponent,
  validateSearch: searchSchema,
});

function RouteComponent() {
  const { category, searchTerm } = Route.useSearch();
  const axiosInstance = useAxios();
  const {
    data: products,
    isLoading,
    error,
  } = useQuery({
    queryKey: ['products'],
    queryFn: () => fetchProducts(axiosInstance, { category, searchTerm }),
    staleTime: 5 * 1000,
  });

  if (isLoading) return <p>Loading...</p>;
  if (error) return <p>Error: {error.message}</p>;

  return (
    <>
      <title>{products ? `${products.length} Products` : 'Home'}</title>

      <h1>Products</h1>
      <p>Search term</p>

      <Row xs={1} md={2} lg={3} xl={4} className="g-3">
        {products &&
          products.map((product) => (
            <Col key={product.id}>
              <ProductCard product={product} imagesUrl={imagesUrl} />
            </Col>
          ))}
      </Row>
    </>
  );
}

import { createFileRoute, Navigate, notFound } from '@tanstack/react-router';
import { useAxios } from '../../../hooks/use-axios';
import { useSuspenseQuery } from '@tanstack/react-query';
import { fetchProduct } from '../../../api';
import { isAxiosError } from 'axios';
import Col from 'react-bootstrap/esm/Col';
import Row from 'react-bootstrap/esm/Row';
import ProductImage from '../../../components/ProductImage';
import AddToCartButton from '../../../components/AddToCartButton';

export const Route = createFileRoute('/_app/products/$productId')({
  component: RouteComponent,

  loader: ({ params: { productId }, context: { axiosInstance, queryClient } }) => {
    queryClient.ensureQueryData({
      queryKey: ['products', productId],
      queryFn: () =>
        fetchProduct(axiosInstance, Number(productId)).catch((error) => {
          if (isAxiosError(error)) {
            if (error.response?.status === 404) throw notFound();
          }
        }),
    });
  },
  notFoundComponent: () => <Navigate to="/" />,
  errorComponent: ErrorComponent,
});

function RouteComponent() {
  const axiosInstance = useAxios();
  const { productId } = Route.useParams();
  const { data: product } = useSuspenseQuery({
    queryKey: ['products', productId],
    queryFn: () => fetchProduct(axiosInstance, Number(productId)),
  });

  return (
    <>
      <Row className="g-5 mb-5 justify-content-center">
        <Col xs={12} md={6} xl={5}>
          <ProductImage
            className="border rounded-3"
            productSrc={product.imageUrl ?? ''}
            fallback="https://placehold.co/640"
            fluid
          />
        </Col>
        <Col xs={12} md={6} xl={5}>
          <h1>{product.name}</h1>
          <p className="text-secondary">{product.sku}</p>

          <p className="fs-3 fw-bold">${product.price}</p>
          <hr />

          <div className="d-flex justify-content-center justify-content-md-start">
            <AddToCartButton product={product} />
          </div>
        </Col>
        <Col xs={12} xl={10}>
          <h3>Description</h3>
          <p>{product.description}</p>
        </Col>
      </Row>
    </>
  );
}

function ErrorComponent() {
  return (
    <Row className="pt-5 justify-content-center">
      <Col className="text-center">
        <p className='display-2'>Oops!</p>
        <p className='fs-5 text-body-secondary'>An unexpected error has ocurred. Please try again later.</p>
      </Col>
    </Row>
  );
}

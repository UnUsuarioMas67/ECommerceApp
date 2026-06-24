import { createFileRoute } from '@tanstack/react-router';
import { useAxios } from '../../hooks/use-axios';
import { useInfiniteQuery } from '@tanstack/react-query';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import Spinner from 'react-bootstrap/Spinner';
import ProductCard from '../../components/ProductCard';
import { fetchProducts, imagesUrl } from '../../api';
import { searchSchema } from '../../schemas';
import { useInView } from 'react-intersection-observer';
import { useEffect } from 'react';

export const Route = createFileRoute('/_app/')({
  component: RouteComponent,
  validateSearch: searchSchema,
});

function RouteComponent() {
  const { category, searchTerm } = Route.useSearch();

  const axiosInstance = useAxios();

  const fetchProductsWithTimeout = (pageParam: number): ReturnType<typeof fetchProducts> => {
    return new Promise((resolve) => {
      setTimeout(() => {
        resolve(fetchProducts(axiosInstance, { category, searchTerm, pageParam }));
      }, 500);
    });
  };

  const { data, status, fetchNextPage, isFetchingNextPage } = useInfiniteQuery({
    queryKey: ['products'],
    queryFn: ({ pageParam }) => fetchProductsWithTimeout(pageParam),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => lastPage.nextPage,
  });

  const { ref, inView } = useInView();

  useEffect(() => {
    if (inView) fetchNextPage();
  }, [inView, fetchNextPage]);

  let title: string | undefined;
  if (searchTerm) title = `'${searchTerm}' - ECommerce`;
  else if (category) title = `${category} - ECommerce`;

  return status === 'pending' ? (
    <div className="d-flex justify-content-center" ref={ref}>
      <Spinner className="my-5" animation="border" role="status" variant="primary">
        <span className="visually-hidden">Loading...</span>
      </Spinner>
    </div>
  ) : status === 'error' ? (
    <p className="text-danger">Oops! Something went wrong.</p>
  ) : (
    <>
      {title && <title>{title}</title>}

      {searchTerm && (
        <p className="text-secondary bg-body-tertiary p-1 px-3 small rounded-pill">
          Showing results for <strong>'{searchTerm}'</strong>
        </p>
      )}
      <h1>Products</h1>

      {data.pages &&
        data.pages.map((page) => (
          <Row xs={1} md={2} lg={3} xl={4} className="g-3 mb-3" key={page.currentPage}>
            {page.data.map((product) => (
              <Col key={product.id}>
                <ProductCard product={product} imagesUrl={imagesUrl} />
              </Col>
            ))}
          </Row>
        ))}

      <div className="d-flex justify-content-center" ref={ref}>
        {isFetchingNextPage && (
          <Spinner className="my-5" animation="border" role="status" variant="primary">
            <span className="visually-hidden">Loading...</span>
          </Spinner>
        )}
      </div>
    </>
  );
}

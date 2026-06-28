import { createFileRoute } from '@tanstack/react-router';
import { useAxios } from '../../hooks/use-axios';
import { useInfiniteQuery, useSuspenseQuery } from '@tanstack/react-query';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import ProductCard from '../../components/ProductCard';
import { fetchCategory, fetchProducts } from '../../api';
import { searchSchema } from '../../schemas';
import { useInView } from 'react-intersection-observer';
import { useEffect } from 'react';
import LoadingSpinner from '../../components/LoadingSpinner';

export const Route = createFileRoute('/_products/')({
  component: RouteComponent,
  validateSearch: searchSchema,
  loaderDeps: ({ search: { category } }) => ({ category }),
  loader: ({ context: { queryClient, axiosInstance }, deps: { category } }) => {
    queryClient.ensureQueryData({
      queryKey: ['category', 'search'],
      queryFn: () => (category ? fetchCategory(axiosInstance, category) : null),
    });
  },
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

  const { data: categoryObj, isError: categoryError } = useSuspenseQuery({
    queryKey: ['category', 'search'],
    queryFn: () => (category ? fetchCategory(axiosInstance, category) : null),
  });

  const { ref, inView } = useInView();

  useEffect(() => {
    if (inView) fetchNextPage();
  }, [inView, fetchNextPage]);

  let title: string | undefined;
  if (searchTerm) title = `'${searchTerm}' - ECommerce`;
  else if (categoryObj) title = `${categoryObj.name} - ECommerce`;

  return status === 'pending' ? (
    <div className="d-flex justify-content-center align-items-center" style={{ height: '100px' }}>
      <LoadingSpinner />
    </div>
  ) : status === 'error' || categoryError ? (
    <p className="text-danger">Oops! Something went wrong.</p>
  ) : (
    <>
      {title && <title>{title}</title>}

      {searchTerm && (
        <p className="text-secondary bg-body-tertiary p-1 px-3 small rounded-3">
          Showing results for <strong className="text-primary-emphasis">'{searchTerm}'</strong>
        </p>
      )}

      <h1>{categoryObj ? categoryObj.name : 'All products'}</h1>

      <Row xs={1} lg={2} className="g-3">
        {data.pages &&
          data.pages
            .map((page) => page.data)
            .flat()
            .map((product) => (
              <Col key={product.id}>
                <ProductCard product={product} />
              </Col>
            ))}
      </Row>

      <div className="d-flex justify-content-center align-items-center" style={{ height: '100px' }} ref={ref}>
        {isFetchingNextPage && <LoadingSpinner />}
      </div>
    </>
  );
}

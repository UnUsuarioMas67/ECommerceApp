import { createFileRoute } from '@tanstack/react-router';
import { useAxios } from '../../hooks/use-axios';
import { useSuspenseInfiniteQuery, useSuspenseQuery } from '@tanstack/react-query';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import ProductCard from '../../components/ProductCard';
import { fetchCategory } from '../../api/categories';
import { fetchProducts } from '../../api/products';
import { searchSchema } from '../../schemas/search';
import { useInView } from 'react-intersection-observer';
import { useEffect } from 'react';
import LoadingSpinner from '../../components/LoadingSpinner';

export const Route = createFileRoute('/_products/')({
  component: RouteComponent,
  validateSearch: searchSchema,
  loaderDeps: ({ search: { category, searchTerm } }) => ({ category, searchTerm }),
  loader: async ({ context: { queryClient, axiosInstance }, deps: { category, searchTerm } }) => {
    await Promise.all([
      queryClient.prefetchQuery({
        queryKey: ['categories', 'search'],
        queryFn: () => (category ? fetchCategory(axiosInstance, category) : null),
        staleTime: Infinity,
      }),
      queryClient.prefetchInfiniteQuery({
        queryKey: ['products'],
        queryFn: ({ pageParam }) => fetchProducts(axiosInstance, { category, searchTerm, pageParam }),
        initialPageParam: 1,
        pages: 1,
        getNextPageParam: (lastPage) => lastPage.nextPage,
        staleTime: 1000 * 60,
      }),
    ]);
  },
  pendingComponent: () => (
    <Row className="justify-content-center mt-5">
      <LoadingSpinner />
    </Row>
  ),
});

function RouteComponent() {
  const { category, searchTerm } = Route.useSearch();
  const axiosInstance = useAxios();

  const { data, error, status, fetchNextPage, isFetchingNextPage } = useSuspenseInfiniteQuery({
    queryKey: ['products'],
    queryFn: ({ pageParam }) => fetchProducts(axiosInstance, { category, searchTerm, pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => lastPage.nextPage,
    staleTime: 1000 * 60,
  });

  const { data: categoryObj, isError: categoryError } = useSuspenseQuery({
    queryKey: ['categories', 'search'],
    queryFn: () => (category ? fetchCategory(axiosInstance, category) : null),
    staleTime: Infinity,
  });

  const { ref, inView } = useInView();

  useEffect(() => {
    if (inView) fetchNextPage();
  }, [inView, fetchNextPage]);

  let title: string | undefined;
  if (searchTerm) title = `'${searchTerm}' - ECommerce`;
  else if (categoryObj) title = `${categoryObj.name} - ECommerce`;

  if (status === 'error') console.log(error);

  return status === 'error' || categoryError ? (
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

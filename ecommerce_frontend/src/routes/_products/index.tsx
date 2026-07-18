import { createFileRoute } from '@tanstack/react-router';
import { useAxios } from '../../hooks/use-axios';
import { infiniteQueryOptions, useSuspenseInfiniteQuery, useSuspenseQuery } from '@tanstack/react-query';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import ProductCard from '../../components/ProductCard';
import { fetchCategory } from '../../api/categories';
import { fetchProducts } from '../../api/products';
import { searchSchema } from '../../schemas/search';
import { useInView } from 'react-intersection-observer';
import { useEffect } from 'react';
import LoadingSpinner from '../../components/LoadingSpinner';
import type { AxiosInstance } from 'axios';
import Title from '../../components/Title';

const productsInfiniteQuery = (axiosInstance: AxiosInstance, category?: string, searchTerm?: string) =>
  infiniteQueryOptions({
    queryKey: ['products'],
    queryFn: ({ pageParam }) => fetchProducts(axiosInstance, { category, searchTerm, pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => (lastPage.page < lastPage.totalPages ? lastPage.page + 1 : null),
    staleTime: 1000 * 60,
  });

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
      queryClient.prefetchInfiniteQuery(productsInfiniteQuery(axiosInstance, category, searchTerm)),
    ]);
  },
  pendingComponent: LoadingSpinner,
});

function RouteComponent() {
  const { category, searchTerm } = Route.useSearch();
  const axiosInstance = useAxios();

  const { data, error, status, fetchNextPage, isFetchingNextPage } = useSuspenseInfiniteQuery(
    productsInfiniteQuery(axiosInstance, category, searchTerm),
  );

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
  if (searchTerm) title = `'${searchTerm}'`;
  else if (categoryObj) title = `${categoryObj.name}`;

  if (status === 'error') console.log(error);

  return status === 'error' || categoryError ? (
    <p className="text-danger">Oops! Something went wrong.</p>
  ) : (
    <>
      <Title text={title} />

      {searchTerm && (
        <p className="text-secondary bg-body-tertiary p-1 px-3 small rounded-3">
          Showing results for <strong className="text-primary-emphasis">'{searchTerm}'</strong>
        </p>
      )}

      <h1>{categoryObj ? categoryObj.name : 'All products'}</h1>

      <Row xs={1} lg={2} className="g-3">
        {data.pages &&
          data.pages
            .map((page) => page.items)
            .flat()
            .map((product) => (
              <Col key={product.id}>
                <ProductCard product={product} />
              </Col>
            ))}
      </Row>

      <div style={{ height: '100px' }} ref={ref}>
        {isFetchingNextPage && <LoadingSpinner />}
      </div>
    </>
  );
}

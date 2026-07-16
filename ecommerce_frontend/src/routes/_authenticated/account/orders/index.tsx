import { createFileRoute } from '@tanstack/react-router';
import LoadingSpinner from '../../../../components/LoadingSpinner';
import { infiniteQueryOptions, useSuspenseInfiniteQuery } from '@tanstack/react-query';
import type { AxiosInstance } from 'axios';
import { fetchOrders } from '../../../../api/orders';
import { useAxios } from '../../../../hooks/use-axios';
import Container from 'react-bootstrap/esm/Container';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import OrdersTable from '../../../../components/OrdersTable';
import { useInView } from 'react-intersection-observer';
import { useEffect } from 'react';

const ordersQuery = (axiosInstance: AxiosInstance) =>
  infiniteQueryOptions({
    queryKey: ['products'],
    queryFn: ({ pageParam }) => fetchOrders(axiosInstance, { pageParam }),
    initialPageParam: 1,
    getNextPageParam: (lastPage) => (lastPage.page < lastPage.totalPages ? lastPage.page + 1 : null),
    staleTime: 1000 * 60,
  });

export const Route = createFileRoute('/_authenticated/account/orders/')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,
  loader: ({ context: { axiosInstance, queryClient } }) => {
    queryClient.prefetchInfiniteQuery(ordersQuery(axiosInstance));
  },
});

function RouteComponent() {
  const axiosInstance = useAxios();
  const { data, status, isFetchingNextPage, fetchNextPage } = useSuspenseInfiniteQuery(ordersQuery(axiosInstance));
  const { ref, inView } = useInView();

  useEffect(() => {
    if (inView) fetchNextPage();
  }, [inView, fetchNextPage]);

  return status === 'error' ? (
    <p className="text-danger">Oops! Something went wrong.</p>
  ) : (
    <Container>
      <Row>
        <Col>
          <h1 className="mb-4">Orders</h1>

          {data.pages.length > 0 ? (
            <OrdersTable data={data} ref={ref} isFetchingNextPage={isFetchingNextPage} />
          ) : (
            <p className="text-body-secondary">No orders have been made yet.</p>
          )}
        </Col>
      </Row>
    </Container>
  );
}

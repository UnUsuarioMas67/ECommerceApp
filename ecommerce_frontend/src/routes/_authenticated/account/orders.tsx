import { createFileRoute } from '@tanstack/react-router';
import LoadingSpinner from '../../../components/LoadingSpinner';
import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import type { AxiosInstance } from 'axios';
import { fetchOrders } from '../../../api/orders';
import type { Order } from '../../../api/types';
import { useAxios } from '../../../hooks/use-axios';
import Container from 'react-bootstrap/esm/Container';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import OrdersTable from '../../../components/OrdersTable';

const ordersQuery = (axiosInstance: AxiosInstance) =>
  queryOptions<Order[]>({
    queryKey: ['orders'],
    queryFn: () => fetchOrders(axiosInstance),
    staleTime: 1000 * 60 * 10,
  });

export const Route = createFileRoute('/_authenticated/account/orders')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,
  loader: ({ context: { axiosInstance, queryClient } }) => {
    queryClient.prefetchQuery(ordersQuery(axiosInstance));
  },
});

function RouteComponent() {
  const axiosInstance = useAxios();
  const { data: orders } = useSuspenseQuery(ordersQuery(axiosInstance));

  return (
    <Container>
      <Row>
        <Col>
          <h1 className="mb-4">Orders</h1>

          {orders.length > 0 ? (
            <OrdersTable orders={orders} />
          ) : (
            <p className="text-body-secondary">No orders have been made yet.</p>
          )}
        </Col>
      </Row>
    </Container>
  );
}

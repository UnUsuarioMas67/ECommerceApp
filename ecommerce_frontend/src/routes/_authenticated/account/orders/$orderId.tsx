import { queryOptions, useSuspenseQuery } from '@tanstack/react-query';
import { createFileRoute, Navigate, notFound } from '@tanstack/react-router';
import type { AxiosInstance } from 'axios';
import { fetchOrder } from '../../../../api/orders';
import { useAxios } from '../../../../hooks/use-axios';
import type { Order } from '../../../../api/types';
import LoadingSpinner from '../../../../components/LoadingSpinner';
import Container from 'react-bootstrap/esm/Container';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import OrderStatusBadge from '../../../../components/OrdersTable/OrderStatusBadge';
import OrderItemsTable from '../../../../components/OrderItemsTable';

const orderQuery = (axiosInstance: AxiosInstance, id: number) =>
  queryOptions<Order>({
    queryKey: ['orders', id],
    queryFn: () =>
      fetchOrder(axiosInstance, id).then((result) => {
        if (!result) throw notFound();

        return result;
      }),
  });

export const Route = createFileRoute('/_authenticated/account/orders/$orderId')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,
  notFoundComponent: () => <Navigate to="/account/orders" />,
  loader: ({ context: { axiosInstance, queryClient }, params: { orderId } }) => {
    queryClient.prefetchQuery(orderQuery(axiosInstance, Number(orderId)));
  },
});

function RouteComponent() {
  const { orderId } = Route.useParams();
  const axiosInstance = useAxios();
  const { data: order } = useSuspenseQuery(orderQuery(axiosInstance, Number(orderId)));
  const { addressLine1, addressLine2, city, country, postalCode, region } = order.address;

  return (
    <Container>
      <Row>
        <Col>
          <dl>
            <dt>Order Id</dt>
            <dd>{order.id}</dd>

            <dt>Date</dt>
            <dd>{order.orderDate}</dd>

            <dt>Total</dt>
            <dd>${order.totalPrice}</dd>

            <dt>Address</dt>
            <dd>{`${addressLine1} ${addressLine2 ? ', ' + addressLine2 : ''}, ${city}, ${region}, ${country}, ${postalCode} `}</dd>

            <dt>Status</dt>
            <dd>
              <OrderStatusBadge status={order.status} />
            </dd>
          </dl>
        </Col>
      </Row>

      <Row>
        <Col>
          <h3>Products overview</h3>
          <OrderItemsTable items={order.items} />
        </Col>
      </Row>
    </Container>
  );
}

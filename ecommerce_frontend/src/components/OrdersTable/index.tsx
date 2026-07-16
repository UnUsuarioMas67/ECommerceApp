import type { Order, PaginatedResponse } from '../../api/types';
import Table from 'react-bootstrap/esm/Table';
import OrderTableRow from './OrderTableRow';
import type { InfiniteData } from '@tanstack/react-query';
import type { useInView } from 'react-intersection-observer';
import LoadingSpinner from '../LoadingSpinner';

type Props = {
  data: InfiniteData<PaginatedResponse<Order>>;
  ref: ReturnType<typeof useInView>['ref'];
  isFetchingNextPage: boolean
};

function OrdersTable({ data, ref, isFetchingNextPage }: Props) {
  return (
    <>
      <Table responsive striped bordered hover>
        <thead>
          <tr>
            <th>Order Id</th>
            <th>Date</th>
            <th>Total</th>
            <th>Address</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {
            data.pages
              .map((page) => page.items)
              .flat()
              .map((order) => <OrderTableRow order={order} key={order.id} />)}
        </tbody>
      </Table>

      <div style={{ height: '100px' }} ref={ref}>
        {isFetchingNextPage && <LoadingSpinner />}
      </div>
    </>
  );
}

export default OrdersTable;

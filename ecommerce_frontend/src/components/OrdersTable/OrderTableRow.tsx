import type { Order } from '../../api/types';
import OrderStatusBadge from './OrderStatusBadge';
import { Link } from '@tanstack/react-router';
import dayjs from 'dayjs'

type Props = {
  order: Order;
};

function OrderTableRow({ order }: Props) {
  const { addressLine1, addressLine2, city, country, postalCode, region } = order.address;

  return (
    <tr>
      <td>{order.id}</td>
      <td>{dayjs(order.orderDate).format('MMM DD, YYYY')}</td>
      <td>${order.totalPrice}</td>
      <td>{`${addressLine1} ${addressLine2 ? ', ' + addressLine2 : ''}, ${city}, ${region}, ${country}, ${postalCode} `}</td>
      <td>
        <OrderStatusBadge status={order.status} />
      </td>
      <td>
        <Link to='/account/orders/$orderId' params={{orderId: order.id.toString()}}>View details</Link>
      </td>
    </tr>
  );
}

export default OrderTableRow;

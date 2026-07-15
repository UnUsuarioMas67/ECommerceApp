import type { OrderStatus } from '../../api/types';
import Badge from 'react-bootstrap/esm/Badge';

type Props = {
  status: OrderStatus;
};

function OrderStatusBadge({ status }: Props) {
  const bgColor = status === 'PENDING' ? 'warning' : status === 'PAID' ? 'success' : 'danger';
  return <Badge bg={bgColor}>{status}</Badge>;
}

export default OrderStatusBadge;

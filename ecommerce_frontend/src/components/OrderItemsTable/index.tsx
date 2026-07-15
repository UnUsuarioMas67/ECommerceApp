import type { OrderItem } from '../../api/types';
import OrderItemsTableRow from './OrderItemsTableRow';
import Table from 'react-bootstrap/esm/Table';

type Props = {
  items: OrderItem[];
};

function OrderItemsTable({ items }: Props) {
  return (
    <Table className="align-middle" responsive striped>
      <thead>
        <tr>
          <th>Product</th>
          <th>Price</th>
          <th>Quantity</th>
          <th>Subtotal</th>
        </tr>
      </thead>
      <tbody>
        {items.map((item) => (
          <OrderItemsTableRow item={item} key={item.productId} />
        ))}
      </tbody>
    </Table>
  );
}

export default OrderItemsTable;

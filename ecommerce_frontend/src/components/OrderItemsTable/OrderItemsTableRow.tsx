import type { OrderItem } from '../../api/types';

type Props = {
  item: OrderItem;
};

function OrderItemsTableRow({ item }: Props) {
  return (
    <tr>
      {/* <td style={{ width: 'fit-content' }}>
        <ProductImage fallback="https://placehold.co/96" productSrc={item.imageUrl ?? ''} style={{ width: 96 }} />
      </td> */}
      <td className="fw-bold">{item.productName}</td>
      <td>${item.unitPrice}</td>
      <td>x{item.quantity}</td>
      <td>${item.subtotal}</td>
    </tr>
  );
}

export default OrderItemsTableRow;

import type { Order } from '../../api/types'
import Table from 'react-bootstrap/esm/Table'
import OrderTableRow from './OrderTableRow'

type Props = {
  orders: Order[]
}

function OrdersTable({orders}: Props) {
  return (
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
        {orders.map(order => <OrderTableRow order={ order } key={order.id} />)}
      </tbody>
    </Table>
  )
}

export default OrdersTable
import { createFileRoute } from '@tanstack/react-router'
import LoadingSpinner from '../../../components/LoadingSpinner'

export const Route = createFileRoute('/_authenticated/account/orders')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,

})

function RouteComponent() {
  return <div>Hello "/_authenticated/account/"!</div>
}

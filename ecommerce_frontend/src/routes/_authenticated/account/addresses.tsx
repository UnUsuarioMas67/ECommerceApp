import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_authenticated/account/addresses')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/_authenticated/account/addresses"!</div>
}

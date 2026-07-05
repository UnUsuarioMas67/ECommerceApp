import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_authenticated/account/settings')({
  component: RouteComponent,
})

function RouteComponent() {
  return <div>Hello "/_authenticated/account/settings"!</div>
}

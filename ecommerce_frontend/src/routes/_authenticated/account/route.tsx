import { createFileRoute, Outlet } from '@tanstack/react-router';

export const Route = createFileRoute('/_authenticated/account')({
  component: RouteComponent,
});

function RouteComponent() {
  return <Outlet />;
}

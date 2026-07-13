import { createFileRoute, Outlet } from '@tanstack/react-router';
import CheckoutErrorProvider from '../../../components/CheckoutErrorProvider';
import LoadingSpinner from '../../../components/LoadingSpinner';

export const Route = createFileRoute('/_authenticated/checkout')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,
});

function RouteComponent() {
  return (
    <CheckoutErrorProvider>
      <Outlet />
    </CheckoutErrorProvider>
  );
}

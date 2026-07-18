import { createFileRoute, Outlet } from '@tanstack/react-router';
import CheckoutErrorProvider from '../../../components/CheckoutErrorProvider';
import LoadingSpinner from '../../../components/LoadingSpinner';
import NotFoundPage from '../../-not-found';

export const Route = createFileRoute('/_authenticated/checkout')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,
  notFoundComponent: NotFoundPage,

});

function RouteComponent() {
  return (
    <CheckoutErrorProvider>
      <Outlet />
    </CheckoutErrorProvider>
  );
}

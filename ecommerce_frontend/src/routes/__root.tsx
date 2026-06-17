import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { createRootRoute, Link, Outlet } from '@tanstack/react-router';
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools';
import AuthProvider from '../components/auth/context/AuthProvider';
import NotFoundLayout from '../layout/NotFoundLayout';

const queryClient = new QueryClient();

const RootLayout = () => (
  <>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <Outlet />
      </AuthProvider>
    </QueryClientProvider>
    <TanStackRouterDevtools />
  </>
);

export const Route = createRootRoute({
  component: RootLayout, 
  notFoundComponent: NotFoundLayout
});

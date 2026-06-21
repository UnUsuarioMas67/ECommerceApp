import { createRootRouteWithContext, Outlet } from '@tanstack/react-router';
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools';
import NotFoundLayout from '../layout/NotFoundLayout';
import type { QueryClient } from '@tanstack/react-query';
import type { useAuth } from '../components/auth/context/AuthContext';
import type { AxiosInstance } from 'axios';

type RouterContext = {
  queryClient: QueryClient;
  authContext: ReturnType<typeof useAuth>
  axiosInstance: AxiosInstance
}

const RootLayout = () => (
  <>
    <Outlet />
    <TanStackRouterDevtools />
  </>
);

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootLayout,
  notFoundComponent: NotFoundLayout,
});

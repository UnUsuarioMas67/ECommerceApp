import { createRootRouteWithContext, Outlet, useMatchRoute } from '@tanstack/react-router';
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools';
import NotFoundLayout from '../layout/NotFoundLayout';
import { useSuspenseQuery, type QueryClient } from '@tanstack/react-query';
import type { useAuth } from '../components/AuthProvider/AuthContext';
import type { AxiosInstance } from 'axios';
import { useAxios } from '../hooks/use-axios';
import { fetchCategories, getCurrentUser } from '../api';
import NavbarComponent from '../components/NavbarComponent';
import Container from 'react-bootstrap/esm/Container';
import FooterComponent from '../components/FooterComponent';

type RouterContext = {
  queryClient: QueryClient;
  authContext: ReturnType<typeof useAuth>;
  axiosInstance: AxiosInstance;
};

const RootLayout = () => {
  const axiosInstance = useAxios();
  const { data: user } = useSuspenseQuery({
    queryKey: ['users', 'me'],
    queryFn: () => getCurrentUser(axiosInstance),
  });
  const { data: categories } = useSuspenseQuery({
    queryKey: ['categories'],
    queryFn: () => fetchCategories(axiosInstance),
  });

  // hide navbar in login and register pages
  const matchRoute = useMatchRoute();
  const hideNavRoutes = ['/login', '/register']
  const matchedNoNavRoute = hideNavRoutes.some((route) => matchRoute({to: route}))

  return (
    <>
      <div className="d-flex flex-column min-vh-100">
        {!matchedNoNavRoute && <NavbarComponent currentUser={user ?? undefined} categories={categories} />}

        <Container as="main" className="flex-grow-1">
          <Outlet />
        </Container>

        <FooterComponent />
      </div>
      
      <TanStackRouterDevtools />
    </>
  );
};

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootLayout,
  notFoundComponent: NotFoundLayout,
  loader: async ({ context: { queryClient, authContext, axiosInstance } }) => {
    if (!(await authContext.ensureLoggedIn())) return;

    await Promise.all([
      queryClient.ensureQueryData({
        queryKey: ['users', 'me'],
        queryFn: () => getCurrentUser(axiosInstance),
      }),
      queryClient.ensureQueryData({
        queryKey: ['categories'],
        queryFn: () => fetchCategories(axiosInstance),
      }),
    ]);
  },
});

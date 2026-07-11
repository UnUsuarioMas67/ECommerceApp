import { createRootRouteWithContext, Outlet, useMatchRoute } from '@tanstack/react-router';
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools';
import { useSuspenseQuery, type QueryClient } from '@tanstack/react-query';
import { useAuth } from '../components/AuthProvider/AuthContext';
import type { AxiosInstance } from 'axios';
import { useAxios } from '../hooks/use-axios';
import { fetchCategories } from '../api/categories';
import NavbarComponent from '../components/NavbarComponent';
import Container from 'react-bootstrap/esm/Container';
import FooterComponent from '../components/FooterComponent';
import ErrorPage from './-error-page';
import NotFoundPage from './-not-found';
import ShoppingCart from '../components/ShoppingCart';
import { fetchCurrentUser } from '../api/user';
import type { useCart } from '../components/CartProvider/CartContext';

type RouterContext = {
  queryClient: QueryClient;
  axiosInstance: AxiosInstance;
  authContext: ReturnType<typeof useAuth>;
  cartContext: ReturnType<typeof useCart>
};

const RootLayout = () => {
  const axiosInstance = useAxios();
  const { data: currentUser } = useSuspenseQuery({
    queryKey: ['users', 'me'],
    queryFn: () => fetchCurrentUser(axiosInstance),
    staleTime: Infinity,
  });
  const { data: categories } = useSuspenseQuery({
    queryKey: ['categories'],
    queryFn: () => fetchCategories(axiosInstance),
    staleTime: Infinity,
    refetchOnWindowFocus: false,
  });

  // hide navbar in login and register pages
  const matchRoute = useMatchRoute();
  const hideNavRoutes = ['/login', '/register'];
  const matchedNoNavRoute = hideNavRoutes.some((route) => matchRoute({ to: route, fuzzy: true }));

  return (
    <>
      <div className="d-flex flex-column min-vh-100">
        {!matchedNoNavRoute && <NavbarComponent user={currentUser} categories={categories} />}

        <Container className="flex-grow-1 p-0 d-flex flex-column" fluid>
          <Outlet />
          <ShoppingCart />
        </Container>

        <FooterComponent small={matchedNoNavRoute} />
      </div>

      <TanStackRouterDevtools />
    </>
  );
};

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootLayout,
  notFoundComponent: NotFoundPage,
  errorComponent: ErrorPage,
  beforeLoad: async ({ context: { authContext } }) => {
    await authContext.ensureLoggedIn();
    
  },
  loader: ({ context: { queryClient, axiosInstance } }) => {
    queryClient.ensureQueryData({
      queryKey: ['users', 'me'],
      queryFn: () => fetchCurrentUser(axiosInstance),
      staleTime: Infinity,
    });
    queryClient.ensureQueryData({
      queryKey: ['categories'],
      queryFn: () => fetchCategories(axiosInstance),
    });
  },
});

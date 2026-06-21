import { createFileRoute, Outlet } from '@tanstack/react-router';
import NavbarComponent from '../../components/NavbarComponent';
import Container from 'react-bootstrap/Container';
import FooterComponent from '../../components/FooterComponent';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useAxios } from '../../hooks/use-axios';
import { getCategories, getCurrentUser } from '../../api';

export const Route = createFileRoute('/_app')({
  component: RouteComponent,
  loader: async ({ context: { queryClient, authContext, axiosInstance } }) => {
    if (!(await authContext.ensureLoggedIn())) return;

    await Promise.all([
      queryClient.ensureQueryData({
        queryKey: ['users', 'me'],
        queryFn: async () => await getCurrentUser(axiosInstance),
      }),
      queryClient.ensureQueryData({
        queryKey: ['categories'],
        queryFn: async () => await getCategories(axiosInstance)
      })
    ])
  },
});

function RouteComponent() {
  const axios = useAxios();
  const { data: user } = useSuspenseQuery({
    queryKey: ['users', 'me'],
    queryFn: async () => await getCurrentUser(axios),
  });

  return (
    <div className="d-flex flex-column min-vh-100">
      <NavbarComponent currentUser={user ?? undefined} />
      <Container as="main" className="flex-grow-1">
        <Outlet />
      </Container>
      <FooterComponent />
    </div>
  );
}

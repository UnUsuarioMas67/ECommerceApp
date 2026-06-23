import { createFileRoute, Outlet } from '@tanstack/react-router';
import NavbarComponent from '../../components/NavbarComponent';
import Container from 'react-bootstrap/Container';
import FooterComponent from '../../components/FooterComponent';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useAxios } from '../../hooks/use-axios';
import { fetchCategories, getCurrentUser } from '../../api';

export const Route = createFileRoute('/_app')({
  component: RouteComponent,
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

function RouteComponent() {
  const axiosInstance = useAxios();
  const { data: user } = useSuspenseQuery({
    queryKey: ['users', 'me'],
    queryFn: () => getCurrentUser(axiosInstance),
  });
  const { data: categories } = useSuspenseQuery({
    queryKey: ['categories'],
    queryFn: () => fetchCategories(axiosInstance),
  });

  return (
    <div className="d-flex flex-column min-vh-100">
      <NavbarComponent currentUser={user ?? undefined} categories={categories} />
      <Container as="main" className="flex-grow-1">
        <Outlet />
      </Container>
      <FooterComponent />
    </div>
  );
}

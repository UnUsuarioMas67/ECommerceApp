import { createFileRoute, Outlet } from '@tanstack/react-router';
import NavbarComponent from '../../components/NavbarComponent';
import Container from 'react-bootstrap/Container';
import FooterComponent from '../../components/FooterComponent';
import { type AxiosInstance } from 'axios';
import type { User } from '../../types/api-types';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useAxios } from '../../hooks/use-axios';

async function getCurrentUser(axios: AxiosInstance) {
  const response = await axios.get<User>('/clients/me', {
    validateStatus: (status) => status === 200 || status === 401,
  });

  if (response.status === 401)
    return null;

  return response.data;
}

export const Route = createFileRoute('/_app')({
  component: RouteComponent,
  loader: async ({ context: { queryClient, authContext, axiosInstance } }) => {
    if (!(await authContext.ensureLoggedIn())) return;

    await queryClient.ensureQueryData({
      queryKey: ['users', 'me'],
      queryFn: async () => await getCurrentUser(axiosInstance),
    });
  },
});

function RouteComponent() {
  const axios = useAxios();
  const { data: user } = useSuspenseQuery({
    queryKey: ['users', 'me'],
    queryFn: async () => await getCurrentUser(axios),
  });

  return (
    <div className="d-flex flex-column vh-100">
      <NavbarComponent currentUser={user ?? undefined} />
      <Container as="main" className="flex-grow-1">
        <Outlet />
      </Container>
      <FooterComponent />
    </div>
  );
}

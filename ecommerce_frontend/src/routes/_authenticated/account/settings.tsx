import { createFileRoute } from '@tanstack/react-router';
import { fetchCurrentUser } from '../../../api/user';
import { useQueryClient, useSuspenseQuery } from '@tanstack/react-query';
import { useAxios } from '../../../hooks/use-axios';
import Container from 'react-bootstrap/esm/Container';
import Row from 'react-bootstrap/esm/Row';
import UpdateUserDataForm from '../../../components/AccountSettings/UpdateUserDataForm';
import LoadingSpinner from '../../../components/LoadingSpinner';
import type { User } from '../../../api/types';
import UpdateUserPasswordForm from '../../../components/AccountSettings/UpdateUserPasswordForm';

export const Route = createFileRoute('/_authenticated/account/settings')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,
  loader: ({ context: { axiosInstance, queryClient } }) => {
    queryClient.ensureQueryData({
      queryKey: ['users', 'me'],
      queryFn: () => fetchCurrentUser(axiosInstance),
    });
  },
});

function RouteComponent() {
  const queryClient = useQueryClient();
  const axiosInstance = useAxios();
  const { data: user } = useSuspenseQuery({
    queryKey: ['users', 'me'],
    queryFn: () => fetchCurrentUser(axiosInstance),
  });

  const onUpdateUserDataSubmitSuccessful = (newUser: User) => {
    queryClient.setQueryData<User>(['users', 'me'], newUser);
  };

  return (
    <Container className="mb-4">
      <h1 className="mb-4">Account settings</h1>

      <Row>
        <h3>User data</h3>
        <UpdateUserDataForm user={user!} onSubmitSuccessful={onUpdateUserDataSubmitSuccessful} />
      </Row>

      <hr className="my-5" />

      <Row>
        <h3>Password</h3>
        <UpdateUserPasswordForm />
      </Row>
    </Container>
  );
}

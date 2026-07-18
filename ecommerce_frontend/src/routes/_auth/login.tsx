import { createFileRoute, Link, useRouter } from '@tanstack/react-router';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import LoginForm from './-login-form';
import type { UserAuth } from '../../api/types';
import { useAuth } from '../../components/AuthProvider/AuthContext';
import Logo from '../../components/Logo';
import Title from '../../components/Title';

export const Route = createFileRoute('/_auth/login')({
  component: RouteComponent,
});

function RouteComponent() {
  const { redirect } = Route.useSearch();
  const navigate = Route.useNavigate();
  const { invalidate: invalidateRouter } = useRouter();
  const { setCredentials } = useAuth();

  const onSubmitSuccessful = (auth: UserAuth) => {
    navigate({ to: redirect ?? '/' });
    setCredentials(auth);
    invalidateRouter();
  };

  return (
    <>
      <Title text='Login'/>

      <Row className="justify-content-center flex-grow-1 g-0 align-items-stretch" as="main">
        <Col lg={8} className="bg-primary text-center pt-5 d-none d-lg-block">
          <Link className="h2 text-light text-decoration-none" to="/">
            <Logo size='lg'/>
          </Link>
        </Col>
        <Col className="p-5 d-flex flex-column justify-content-start justify-content-lg-center border-start">
          <Link className="h2 text-center text-decoration-none d-lg-none" to="/" style={{ marginBottom: '100px' }}>
            <Logo size='lg'/>
          </Link>
          <h1 className="text-center text-body mb-5">Sign In</h1>
          <LoginForm onSubmitSuccessful={onSubmitSuccessful} />
          <span className="text-center small">
            Don't have an account?{' '}
            <Link to="/register" search={{ redirect, passRedirectToLogin: true }}>
              Register
            </Link>
          </span>
        </Col>
      </Row>
    </>
  );
}

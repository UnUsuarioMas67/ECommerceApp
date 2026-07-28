import { createFileRoute, Link } from '@tanstack/react-router';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import { ChevronLeft } from 'react-bootstrap-icons';
import RegisterForm from './-register-form';
import z from 'zod';
import Title from '../../components/Title';

export const Route = createFileRoute('/_auth/register')({
  component: RouteComponent,
  validateSearch: z.object({
    passRedirectToLogin: z.boolean().optional(),
  }),
});

function RouteComponent() {
  const { redirect, passRedirectToLogin } = Route.useSearch();
  const navigate = Route.useNavigate();

  const redirectParams = () => {
    if (redirect && !passRedirectToLogin) return { to: redirect };
    else if (redirect && passRedirectToLogin) return { to: '/login', search: { redirect } };
    else return { to: '/login' };
  };

  const onSubmitSuccessful = () => {
    navigate(redirectParams());
  };

  return (
    <>
      <Title text="Register" />

      <Row as="main" className="flex-grow-1 justify-content-center align-items-center p-3 p-md-0">
        <Col md={6} xl={4}>
          <Link {...redirectParams()} className="btn btn-sm btn-outline-secondary border-0 rounded-circle mb-2 p-2">
            <ChevronLeft size={24} />
          </Link>
          <h1 className="text-center text-body mb-5">Create a new account</h1>
          <RegisterForm onSubmitSuccessful={onSubmitSuccessful} />
        </Col>
      </Row>
    </>
  );
}

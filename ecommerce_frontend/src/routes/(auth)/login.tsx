import { createFileRoute, Link } from '@tanstack/react-router';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import LoginForm from './-login-form';

export const Route = createFileRoute('/(auth)/login')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <>
      <Row className="justify-content-center flex-grow-1 g-0 align-items-stretch" as="main">
        <Col lg={8} className='bg-primary text-center pt-5 d-none d-lg-block'>
          <Link className='h2 text-light text-decoration-none' to="/">
            ECommerce
          </Link>
        </Col>
        <Col className='p-5 d-flex flex-column justify-content-start justify-content-lg-center border-start'>
          <Link className='h2 text-center text-decoration-none d-lg-none' to="/" style={{marginBottom: '100px'}}>
            ECommerce
          </Link>
          <h1 className="text-center text-body mb-5">Sign Up</h1>
          <LoginForm />
          <span className='text-center small'>Don't have an account? <Link to='/register'>Register</Link></span>
        </Col>
      </Row>
    </>
  );
}

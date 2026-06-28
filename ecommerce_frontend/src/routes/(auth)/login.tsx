import { createFileRoute, Link, useNavigate } from '@tanstack/react-router';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { loginSchema, type LoginRequest } from '../../schemas';
import { zodResolver } from '@hookform/resolvers/zod';
import { useLogin } from '../../hooks/account';

import Container from 'react-bootstrap/esm/Container';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import Card from 'react-bootstrap/esm/Card';
import LoginForm from './-login-form';

export const Route = createFileRoute('/(auth)/login')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <>
      <Row className="justify-content-center flex-grow-1 g-0 align-items-stretch" as="main">
        <Col lg={8} className='bg-primary text-center pt-5'>
          <Link className='h2 text-light text-decoration-none' to="/">
            ECommerce
          </Link>
        </Col>
        <Col className='p-5 d-flex flex-column justify-content-center border-start'>
          <h1 className="text-center text-body mb-5">Sign Up</h1>
          <LoginForm />
          <span className='text-center small'>Don't have an account? <Link to='/register'>Register</Link></span>
        </Col>
      </Row>
    </>
  );
}

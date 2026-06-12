import { loginSchema, type LoginRequest } from '../../schemas/account';
import { zodResolver } from '@hookform/resolvers/zod';
import { useForm } from 'react-hook-form';
import { useLogin } from '../../hooks/user';
import { useNavigate } from 'react-router';
import { isAxiosError } from 'axios';

import Container from 'react-bootstrap/Container';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import Alert from 'react-bootstrap/Alert';

import type { SubmitHandler } from 'react-hook-form';
function handleLoginErrorMsg(error: Error): string {
  if (isAxiosError(error)) {
    if (error.response) {
      if (error.response.status === 401) return 'Incorrect email or password.';

      if (error.response.status >= 500) return 'Server error. Please try again later.';

      return 'Login failed due to an unexpected error.';
    }

    return 'No response from server.';
  }

  throw error;
}

function LoginForm() {
  const navigate = useNavigate();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setError,
  } = useForm<LoginRequest>({
    resolver: zodResolver(loginSchema),
  });

  const { mutate, isPending, isSuccess } = useLogin();

  const onSubmit: SubmitHandler<LoginRequest> = async (data) => {
    mutate(data, {
      onSuccess: () => {
        navigate('/protected-name');
      },
      onError: (error) => {
        setError('root', { message: handleLoginErrorMsg(error) });
      },
    });
  };

  return (
    <Container>
      <h1>Login</h1>

      {errors.root && <Alert variant="danger">{errors.root.message}</Alert>}
      {isSuccess && <Alert variant="success">Login Successful!</Alert>}

      <Form onSubmit={handleSubmit(onSubmit)}>
        <Form.Group className="mb-3" controlId="email">
          <Form.Label>Email</Form.Label>
          <Form.Control type="email" placeholder="Enter email" {...register('email')} isInvalid={!!errors.email} />

          <Form.Control.Feedback type="invalid">{errors.email?.message}</Form.Control.Feedback>
        </Form.Group>

        <Form.Group className="mb-3" controlId="password">
          <Form.Label>Password</Form.Label>
          <Form.Control
            type="password"
            placeholder="Enter password"
            {...register('password')}
            isInvalid={!!errors.password}
          />

          <Form.Control.Feedback type="invalid">{errors.password?.message}</Form.Control.Feedback>
        </Form.Group>

        <Button variant="primary" type="submit" disabled={isPending}>
          {isPending ? 'Logging in...' : 'Login'}
        </Button>
      </Form>
    </Container>
  );
}

export default LoginForm;

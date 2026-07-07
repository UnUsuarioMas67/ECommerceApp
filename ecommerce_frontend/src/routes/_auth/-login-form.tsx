import { zodResolver } from '@hookform/resolvers/zod';
import Alert from 'react-bootstrap/esm/Alert';
import Button from 'react-bootstrap/esm/Button';
import FloatingLabel from 'react-bootstrap/esm/FloatingLabel';
import Form from 'react-bootstrap/esm/Form';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { type LoginRequest, loginSchema } from '../../schemas/auth';
import { isAxiosError } from 'axios';
import { useMutation } from '@tanstack/react-query';
import { postLogin } from '../../api/user';
import { useAxios } from '../../hooks/use-axios';
import type { UserAuth } from '../../api/types';

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

type Props = {
  onSubmitSuccessful: (auth: UserAuth) => void;
};

function LoginForm({ onSubmitSuccessful }: Props) {
  const axiosInstance = useAxios();

  const {
    register,
    handleSubmit,
    formState: { errors },
    setError,
  } = useForm<LoginRequest>({
    resolver: zodResolver(loginSchema),
  });

  const { mutate, isPending } = useMutation<UserAuth, Error, LoginRequest>({
    mutationFn: (data) => postLogin(axiosInstance, data),
    onSuccess: (auth) => {
      onSubmitSuccessful(auth);
    },
    onError: (error) => {
      setError('root', { message: handleLoginErrorMsg(error) });
    },
  });

  const onSubmit: SubmitHandler<LoginRequest> = (data) => {
    mutate(data);
  };

  return (
    <>
      {errors.root && <Alert variant="danger">{errors.root.message}</Alert>}

      <Form onSubmit={handleSubmit(onSubmit)}>
        <FloatingLabel controlId="floatingInput" label="Email address" className="mb-3">
          <Form.Control type="email" placeholder="name@example.com" {...register('email')} isInvalid={!!errors.email} />
          <Form.Control.Feedback type="invalid">{errors.email?.message}</Form.Control.Feedback>
        </FloatingLabel>

        <FloatingLabel controlId="floatingPassword" label="Password">
          <Form.Control
            type="password"
            placeholder="Password"
            {...register('password')}
            isInvalid={!!errors.password}
          />
          <Form.Control.Feedback type="invalid">{errors.password?.message}</Form.Control.Feedback>
        </FloatingLabel>

        <Button className="w-100 mt-5" variant="primary" type="submit" disabled={isPending}>
          {isPending ? 'Logging in...' : 'Login'}
        </Button>
      </Form>
    </>
  );
}

export default LoginForm;

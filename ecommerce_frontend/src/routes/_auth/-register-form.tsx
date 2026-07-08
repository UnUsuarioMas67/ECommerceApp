import { zodResolver } from '@hookform/resolvers/zod';
import Alert from 'react-bootstrap/esm/Alert';
import Button from 'react-bootstrap/esm/Button';
import FloatingLabel from 'react-bootstrap/esm/FloatingLabel';
import Form from 'react-bootstrap/esm/Form';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { isAxiosError } from 'axios';
import { useMutation } from '@tanstack/react-query';
import { useAxios } from '../../hooks/use-axios';
import type { Api422ErrorBody, User } from '../../api/types';
import { registerSchema, type RegisterRequest } from '../../schemas/user';
import { postRegister } from '../../api/user';
import Col from 'react-bootstrap/esm/Col';
import Row from 'react-bootstrap/esm/Row';

function handleRegisterErrorMsg(error: Error): string | Api422ErrorBody {
  if (isAxiosError(error)) {
    if (error.response) {
      if (error.response.status === 422) return error.response.data as Api422ErrorBody;

      if (error.response.status >= 500) return 'Server error. Please try again later.';

      return 'Registration failed due to an unexpected error.';
    }

    return 'No response from server.';
  }

  throw error;
}

type Props = {
  onSubmitSuccessful: (user: User) => void;
};

function RegisterForm({ onSubmitSuccessful }: Props) {
  const axiosInstance = useAxios();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<RegisterRequest>({
    resolver: zodResolver(registerSchema),
  });

  const { mutate, isPending } = useMutation<User, Error, RegisterRequest>({
    mutationFn: (data) => postRegister(axiosInstance, data),
    onSuccess: (data) => {
      onSubmitSuccessful(data);
    },
    onError: (error) => {
      const registerError = handleRegisterErrorMsg(error);

      if (typeof registerError === 'string') setError('root', { message: registerError });
      else if (registerError.errorType === 'email_already_used') setError('email', { message: registerError.message });
      else if (registerError.errorType === 'phone_already_used')
        setError('phoneNumber', { message: registerError.message });
    },
  });

  const onSubmit: SubmitHandler<RegisterRequest> = (data) => {
    mutate(data);
  };

  return (
    <>
      {errors.root && <Alert variant="danger">{errors.root.message}</Alert>}

      <Form onSubmit={handleSubmit(onSubmit)}>
        <Row className="g-3">
          <Col md>
            <FloatingLabel controlId="floatingFirstName" label="First name" className="mb-3">
              <Form.Control placeholder="Your first name" {...register('firstName')} isInvalid={!!errors.firstName} />
              <Form.Control.Feedback type="invalid">{errors.firstName?.message}</Form.Control.Feedback>
            </FloatingLabel>
          </Col>
          <Col md>
            <FloatingLabel controlId="floatingLastName" label="Last name" className="mb-3">
              <Form.Control placeholder="Your last name" {...register('lastName')} isInvalid={!!errors.lastName} />
              <Form.Control.Feedback type="invalid">{errors.lastName?.message}</Form.Control.Feedback>
            </FloatingLabel>
          </Col>
        </Row>

        <FloatingLabel controlId="floatingEmail" label="Email address" className="mb-3">
          <Form.Control
            type="email"
            placeholder="name@example.com"
            {...register('email')}
            isInvalid={!!errors.email}
            autoComplete="off"
          />
          <Form.Control.Feedback type="invalid">{errors.email?.message}</Form.Control.Feedback>
        </FloatingLabel>

        <FloatingLabel controlId="floatingTel" label="Phone number" className="mb-3">
          <Form.Control
            type="tel"
            placeholder="Phone number"
            {...register('phoneNumber')}
            isInvalid={!!errors.phoneNumber}
          />
          <Form.Control.Feedback type="invalid">{errors.phoneNumber?.message}</Form.Control.Feedback>
        </FloatingLabel>

        <FloatingLabel controlId="floatingDate" label="Birth date" className="mb-3">
          <Form.Control
            type="date"
            placeholder="Birth date"
            {...register('birthDate')}
            isInvalid={!!errors.birthDate}
          />
          <Form.Control.Feedback type="invalid">{errors.birthDate?.message}</Form.Control.Feedback>
        </FloatingLabel>

        <FloatingLabel controlId="floatingPassword" label="Password" className="mb-3">
          <Form.Control
            type="password"
            placeholder="Password"
            {...register('password')}
            isInvalid={!!errors.password}
            autoComplete="new-password"
          />
          <Form.Control.Feedback type="invalid">{errors.password?.message}</Form.Control.Feedback>
        </FloatingLabel>

        <FloatingLabel controlId="floatingPasswordConfirm" label="Confirm password">
          <Form.Control
            type="password"
            placeholder="Confirm password"
            {...register('passwordConfirm')}
            isInvalid={!!errors.passwordConfirm}
            autoComplete="off"
          />
          <Form.Control.Feedback type="invalid">{errors.passwordConfirm?.message}</Form.Control.Feedback>
        </FloatingLabel>

        <Button className="w-100 mt-5" variant="primary" type="submit" disabled={isPending}>
          {isPending || isSubmitting ? 'Creating account...' : 'Register'}
        </Button>
      </Form>
    </>
  );
}

export default RegisterForm;

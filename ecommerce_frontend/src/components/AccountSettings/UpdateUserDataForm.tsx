import type { Api422ErrorBody, User } from '../../api/types';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { isAxiosError } from 'axios';
import Alert from 'react-bootstrap/esm/Alert';
import Button from 'react-bootstrap/esm/Button';
import Col from 'react-bootstrap/esm/Col';
import Row from 'react-bootstrap/esm/Row';
import Form from 'react-bootstrap/esm/Form';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { updateUser } from '../../api/user';
import { useAxios } from '../../hooks/use-axios';
import { userDataUpdateSchema, type UserDataUpdate } from '../../schemas/user';
import { PencilFill } from 'react-bootstrap-icons';
import { useEffect, useState } from 'react';

function handleErrorMessage(error: Error): string | Api422ErrorBody {
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
  user: User
  onSubmitSuccessful?: (user: User) => void;
};

function UpdateUserDataForm({ user, onSubmitSuccessful }: Props) {
  const [disabled, setDisabled] = useState(true);

  const axiosInstance = useAxios();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError, reset
  } = useForm<UserDataUpdate>({
    resolver: zodResolver(userDataUpdateSchema), defaultValues: user
  });

  const { mutate, isPending, isSuccess } = useMutation<User, Error, UserDataUpdate>({
    mutationFn: (data) => updateUser(axiosInstance, data),
    onSuccess: (data) => {
      onSubmitSuccessful?.(data);
      setDisabled(true);
    },
    onError: (error) => {
      const registerError = handleErrorMessage(error);

      if (typeof registerError === 'string') setError('root', { message: registerError });
      else if (registerError.errorType === 'phone_already_used')
        setError('phoneNumber', { message: registerError.message });
    },
  });

  const onSubmit: SubmitHandler<UserDataUpdate> = (data) => {
    mutate(data);
  };

  useEffect(() => {
    reset(user)
  }, [user, reset])

  return (
    <>
      {disabled && (
        <Button variant="link" className="text-start mb-3" onClick={() => setDisabled(false)}>
          <PencilFill /> Update
        </Button>
      )}

      {isSuccess && <Alert variant='success'>Account successfully updated</Alert>}
      {errors.root && <Alert variant="danger">{errors.root.message}</Alert>}

      <Form onSubmit={handleSubmit(onSubmit)}>
        <fieldset disabled={disabled}>
          <Row className="g-3">
            <Col md>
              <Form.Group className="mb-3" controlId="firstName">
                <Form.Label>First name</Form.Label>
                <Form.Control placeholder="Your first name" {...register('firstName')} isInvalid={!!errors.firstName} />
                <Form.Control.Feedback type="invalid">{errors.firstName?.message}</Form.Control.Feedback>
              </Form.Group>
            </Col>

            <Col md>
              <Form.Group className="mb-3" controlId="lastName">
                <Form.Label>Last name</Form.Label>
                <Form.Control placeholder="Your last name" {...register('lastName')} isInvalid={!!errors.lastName} />
                <Form.Control.Feedback type="invalid">{errors.lastName?.message}</Form.Control.Feedback>
              </Form.Group>
            </Col>
          </Row>

          <Form.Group className="mb-3" controlId="phoneNumber">
            <Form.Label>Phone number</Form.Label>
            <Form.Control
              type="tel"
              placeholder="+11111111111 "
              {...register('phoneNumber')}
              isInvalid={!!errors.phoneNumber}
            />
            <Form.Control.Feedback type="invalid">{errors.phoneNumber?.message}</Form.Control.Feedback>
          </Form.Group>

          <Form.Group className="mb-3" controlId="birthDate">
            <Form.Label>Birth date</Form.Label>
            <Form.Control
              type="date"
              placeholder="Birth date"
              {...register('birthDate')}
              isInvalid={!!errors.birthDate}
            />
            <Form.Control.Feedback type="invalid">{errors.birthDate?.message}</Form.Control.Feedback>
          </Form.Group>

          {!disabled && <div className='mt-4'>
            <Button variant="primary" type="submit" disabled={isPending}>
              {isPending || isSubmitting ? 'Saving...' : 'Save changes'}
            </Button>
            <Button className="ms-2" variant="secondary" onClick={() => setDisabled(true)}>
              Cancel
            </Button>
          </div>}
        </fieldset>
      </Form>
    </>
  );
}

export default UpdateUserDataForm;

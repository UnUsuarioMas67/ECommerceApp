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
import {
  userDataUpdateSchema,
  userPasswordUpdateSchema,
  type UserDataUpdate,
  type UserPasswordUpdate,
} from '../../schemas/user';
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
  onSubmitSuccessful?: (user: User) => void;
};

function UpdateUserPasswordForm({ onSubmitSuccessful }: Props) {
  const [disabled, setDisabled] = useState(true);

  const axiosInstance = useAxios();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<UserPasswordUpdate>({
    resolver: zodResolver(userPasswordUpdateSchema),
  });

  const { mutate, isPending, isSuccess } = useMutation<User, Error, UserPasswordUpdate>({
    mutationFn: (data) => updateUser(axiosInstance, data),
    onSuccess: (data) => {
      onSubmitSuccessful?.(data);
      setDisabled(true);
    },
    onError: (error) => {
      const registerError = handleErrorMessage(error);

      if (typeof registerError === 'string') setError('root', { message: registerError });
      else if (registerError.errorType === 'password_update')
        setError('passwordUpdate.oldPassword', { message: registerError.message });
    },
  });

  const onSubmit: SubmitHandler<UserPasswordUpdate> = (data) => {
    mutate(data);
  };

  return (
    <>
      {disabled && (
        <Button variant="link" className="text-start mb-3" onClick={() => setDisabled(false)}>
          <PencilFill /> Change password
        </Button>
      )}

      {isSuccess && <Alert variant='success'>Password successfully changed</Alert>}
      {errors.root && <Alert variant="danger">{errors.root.message}</Alert>}

      <Form onSubmit={handleSubmit(onSubmit)}>
        <fieldset disabled={disabled}>
          <Form.Group className="mb-3" controlId="oldPassword">
            <Form.Label>Old password</Form.Label>
            <Form.Control
              type="password"
              {...register('passwordUpdate.oldPassword')}
              isInvalid={!!errors.passwordUpdate?.oldPassword}
              autoComplete='off'
            />
            <Form.Control.Feedback type="invalid">{errors.passwordUpdate?.oldPassword?.message}</Form.Control.Feedback>
          </Form.Group>

          <Form.Group className="mb-3" controlId="newPassword">
            <Form.Label>New password</Form.Label>
            <Form.Control
              type="password"
              {...register('passwordUpdate.newPassword')}
              isInvalid={!!errors.passwordUpdate?.newPassword}
              autoComplete='off'
            />
            <Form.Control.Feedback type="invalid">{errors.passwordUpdate?.newPassword?.message}</Form.Control.Feedback>
          </Form.Group>

          <Form.Group className="mb-3" controlId="passwordConfirm">
            <Form.Label>Confirm password</Form.Label>
            <Form.Control
              type="password"
              {...register('passwordUpdate.passwordConfirm')}
              isInvalid={!!errors.passwordUpdate?.passwordConfirm}
              autoComplete='off'
            />
            <Form.Control.Feedback type="invalid">{errors.passwordUpdate?.passwordConfirm?.message}</Form.Control.Feedback>
          </Form.Group>

          {!disabled && (
            <div className="mt-4">
              <Button variant="primary" type="submit" disabled={isPending}>
                {isPending || isSubmitting ? 'Saving...' : 'Save changes'}
              </Button>
              <Button className="ms-2" variant="secondary" onClick={() => setDisabled(true)}>
                Cancel
              </Button>
            </div>
          )}
        </fieldset>
      </Form>
    </>
  );
}

export default UpdateUserPasswordForm;

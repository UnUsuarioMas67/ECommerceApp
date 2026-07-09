import z from 'zod';
import { required, lengthError, invalid } from './helpers';

const email = z.email({ error: (iss) => (iss.input === '' ? required('Email') : invalid('Email address')) });
const loginPassword = z.string().min(1, required('Password'));

const firstName = z
  .string()
  .min(1, required('First name'))
  .max(50, lengthError('First name', 50, 'max'));
const lastName = z
  .string()
  .min(1, required('Last name'))
  .max(50, lengthError('Last name', 50, 'max'));

const phoneNumber = z.e164({ error: (iss) => (iss.input === '' ? required('Phone number') : invalid('Phone number')) });
const birthDate = z.iso.date({ error: (iss) => (iss.input === '' ? required('Birth date') : invalid('Birth date')) });

const password = z
  .string()
  .min(8, { error: (iss) => (iss.input === '' ? required('Password') : lengthError('Password', 8, 'min')) });
const passwordConfirm = z.string().min(1, { error: required('Confirm password') });

export const loginSchema = z.object({
  email,
  password: loginPassword,
});

export type LoginRequest = z.infer<typeof loginSchema>;

export const registerSchema = z
  .object({
    firstName,
    lastName,
    email,
    phoneNumber,
    birthDate,
    password,
    passwordConfirm,
  })
  .refine((data) => data.password === data.passwordConfirm, {
    error: "Passwords don't match",
    path: ['passwordConfirm'],
  })
  .refine(
    (data) => {
      const parsedDate = Date.parse(data.birthDate);
      return parsedDate < Date.now();
    },
    {
      error: "Birth date can't be in the future",
      path: ['birthDate'],
    },
  );

export type RegisterRequest = z.infer<typeof registerSchema>;

export const userPasswordUpdateSchema = z.object({
  passwordUpdate: z
    .object({ oldPassword: loginPassword, newPassword: password, passwordConfirm })
    .refine((data) => data.newPassword === data.passwordConfirm, {
      error: "Passwords don't match",
      path: ['passwordConfirm'],
    }),
});

export type UserPasswordUpdate = z.infer<typeof userPasswordUpdateSchema>;

export const userDataUpdateSchema = z
  .object({
    firstName,
    lastName,
    phoneNumber,
    birthDate,
  })
  .refine(
    (data) => {
      if (!data.birthDate) return true;

      const parsedDate = Date.parse(data.birthDate);
      return parsedDate < Date.now();
    },
    {
      error: "Birth date can't be in the future",
      path: ['birthDate'],
    },
  );

export type UserDataUpdate = z.infer<typeof userDataUpdateSchema>;

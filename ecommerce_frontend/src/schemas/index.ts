import z from 'zod';
import { required, lengthError } from './helpers';

export const loginSchema = z.object({
  email: z.email({ error: (iss) => (iss.input === '' ? 'Email is required' : 'Invalid email address') }),
  password: z.string().min(1, 'Password is required'),
});

export type LoginRequest = z.infer<typeof loginSchema>;

export const registerSchema = z
  .object({
    firstName: z
      .string()
      .min(1, required('First name'))
      .max(50, lengthError('First name', 50, 'max')),
    lastName: z
      .string()
      .min(1, required('Last name'))
      .max(50, lengthError('Last name', 50, 'max')),
    email: z.email({ error: (iss) => (iss.input === '' ? required('Email') : 'Invalid email address') }),
    password: z
      .string()
      .min(1, required('Password'))
      .max(8, lengthError('Password', 8, 'max')),
    passwordConfirm: z.string().min(1, required('Confirm password')),
    phoneNumber: z.e164({ error: (iss) => (iss.input === '' ? required('Phone number') : 'Invalid phone number') }),
    birthDate: z.iso.date({ error: (iss) => (iss.input === '' ? 'Birth date is required' : 'Invalid date') }),
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

export const searchSchema = z.object({
  searchTerm: z.string().optional(),
  category: z.string().optional(),
});

export type ProductSearch = z.infer<typeof searchSchema>;

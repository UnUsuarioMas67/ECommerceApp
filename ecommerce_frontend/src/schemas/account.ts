import z from 'zod';
import { required, lengthError } from './helpers';

export const loginSchema = z.object({
  email: z.email({ error: (iss) => (iss.input === '' ? 'Email is required' : 'Invalid email address') }),
  password: z.string().min(1, 'Password is required'),
});

export type LoginRequest = z.infer<typeof loginSchema>;

export const registerSchema = z.object({
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
});

export type RegisterRequest = z.infer<typeof registerSchema>;
import z from 'zod';
import { lengthError, required } from './helpers';

const addressLine1 = z
  .string()
  .min(1, { error: required('Address line 1') })
  .max(100, { error: lengthError('Address line 1', 100, 'max') });

const addressLine2 = z.string().max(100, { error: lengthError('Address line 2', 100, 'max') });

const region = z
  .string()
  .min(1, { error: required('Region') })
  .max(100, { error: lengthError('Region', 100, 'max') });

const city = z
  .string()
  .min(1, { error: required('City') })
  .max(100, { error: lengthError('City', 100, 'max') });

const postalCode = z
  .string()
  .min(1, { error: required('Postal code') })
  .max(10, { error: lengthError('Postal code', 10, 'max') });

const countryCode = z.string().length(2, { error: required('Country') });

export const addressCreateSchema = z.object({
  addressLine1,
  addressLine2: addressLine2.optional(),
  region,
  city,
  postalCode,
  countryCode,
});

export type AddressCreate = z.infer<typeof addressCreateSchema>;

export const addressUpdateSchema = z.object({
  addressLine1,
  addressLine2,
  region,
  city,
  postalCode,
  countryCode,
});

export type AddressUpdate = z.infer<typeof addressUpdateSchema>;

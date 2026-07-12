import z from 'zod';

const cartItemSchema = z.object({
  productId: z.int().positive(),
  quantity: z.int().positive(),
});

const cartSchema = z.object({
  items: z.array(cartItemSchema).min(1),
});

export const checkoutSchema = z.object({
  addressId: z.int().positive(),
  cart: cartSchema,
});

export type CheckoutRequest = z.infer<typeof checkoutSchema>;
export type CartItemRequest = z.infer<typeof cartItemSchema>
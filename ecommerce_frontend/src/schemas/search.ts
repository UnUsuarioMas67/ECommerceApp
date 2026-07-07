import z from "zod";

export const searchSchema = z.object({
  searchTerm: z.string().optional(),
  category: z.string().optional(),
});

export type ProductSearch = z.infer<typeof searchSchema>;
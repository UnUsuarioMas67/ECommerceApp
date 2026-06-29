import { createFileRoute, Outlet } from '@tanstack/react-router';
import z from 'zod';

export const Route = createFileRoute('/_auth')({
  component: RouteComponent,
  validateSearch: z.object({
    redirect: z.string().optional(),
  }),
});

function RouteComponent() {
  return <Outlet />;
}

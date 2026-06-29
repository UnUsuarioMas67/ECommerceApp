import { createFileRoute, Outlet, redirect } from '@tanstack/react-router';
import z from 'zod';

export const Route = createFileRoute('/_auth')({
  component: RouteComponent,
  validateSearch: z.object({
    redirect: z.string().optional(),
  }),
  beforeLoad: async ({context: {authContext} }) => {
    if (await authContext.ensureLoggedIn())
      throw redirect({to: '/'})
  }
});

function RouteComponent() {
  return <Outlet />;
}

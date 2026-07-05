import { createFileRoute, Outlet, redirect } from '@tanstack/react-router';

export const Route = createFileRoute('/_authenticated')({
  component: RouteComponent,
  beforeLoad: async ({ context: { authContext }, location: { href } }) => {
    if (!(await authContext.ensureLoggedIn())) {
      throw redirect({ to: '/login', search: { redirect: href } });
    }
  },
});

function RouteComponent() {
  return <Outlet />;
}

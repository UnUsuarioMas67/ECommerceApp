import { createFileRoute, Outlet } from '@tanstack/react-router';
import Container from 'react-bootstrap/esm/Container';

export const Route = createFileRoute('/_products')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <Container as="main">
      <Outlet />
    </Container>
  );
}

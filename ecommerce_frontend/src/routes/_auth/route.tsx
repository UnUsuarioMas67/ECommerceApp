import { createFileRoute, Outlet } from '@tanstack/react-router';
import Container from 'react-bootstrap/Container'


export const Route = createFileRoute('/_auth')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <Container>
      <Outlet />
    </Container>
  );
}

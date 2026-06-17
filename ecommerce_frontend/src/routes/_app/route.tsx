import { createFileRoute, Outlet } from '@tanstack/react-router';
import NavbarComponent from '../../components/Navbar';
import Container from 'react-bootstrap/Container';

export const Route = createFileRoute('/_app')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <>
      <NavbarComponent />
      <Container>
        <Outlet />
      </Container>
    </>
  );
}

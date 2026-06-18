import { createFileRoute, Outlet } from '@tanstack/react-router';
import NavbarComponent from '../../components/NavbarComponent';
import Container from 'react-bootstrap/Container';
import FooterComponent from '../../components/FooterComponent';

export const Route = createFileRoute('/_app')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <>
      <NavbarComponent />
      <Container as="main">
        <Outlet />
      </Container>
      <FooterComponent />
    </>
  );
}

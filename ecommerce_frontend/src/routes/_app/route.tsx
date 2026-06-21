import { createFileRoute, Outlet } from '@tanstack/react-router';
import NavbarComponent from '../../components/NavbarComponent';
import Container from 'react-bootstrap/Container';
import FooterComponent from '../../components/FooterComponent';

export const Route = createFileRoute('/_app')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <div className='d-flex flex-column vh-100'>
      <NavbarComponent />
      <Container as="main" className='flex-grow-1'>
        <Outlet />
      </Container>
      <FooterComponent />
    </div>
  );
}

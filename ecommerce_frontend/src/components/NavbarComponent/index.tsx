import Container from 'react-bootstrap/Container';
import Navbar from 'react-bootstrap/Navbar';
import { Link } from '@tanstack/react-router';
import type { User } from '../../api/types';
import ProductSearchForm from './ProductSearchForm';
import NavbarNav from './NavbarNav';

type Props = {
  currentUser?: User;
};

function NavbarComponent({ currentUser }: Props) {
  return (
    <Navbar sticky="top" expand="lg" className="bg-primary mb-4" data-bs-theme="dark">
      <Container fluid className="px-5">
        <Navbar.Brand as={Link} to="/" className="fw-bold text-white">
          ECommerce
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="basic-navbar-nav" />

        <Navbar.Collapse id="basic-navbar-nav">
          <ProductSearchForm />
          <NavbarNav currentUser={currentUser} />
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;

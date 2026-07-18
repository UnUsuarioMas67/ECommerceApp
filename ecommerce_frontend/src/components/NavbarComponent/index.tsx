import Container from 'react-bootstrap/Container';
import Navbar from 'react-bootstrap/Navbar';
import type { Category, User } from '../../api/types';
import ProductSearchForm from './ProductSearchForm';
import NavbarNav from './NavbarNav';
import { Link } from '@tanstack/react-router';
import Logo from '../Logo';

type Props = {
  user: User | null;
  categories: Category[];
};

function NavbarComponent({ user: currentUser, categories }: Props) {
  return (
    <Navbar sticky="top" expand="lg" className="bg-primary mb-4" data-bs-theme="dark">
      <Container fluid className="px-2 px-sm-5">
        <Navbar.Brand as={Link} className="fw-bold text-white" to="/" reloadDocument>
          <Logo />
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="basic-navbar-nav" />

        <Navbar.Collapse id="basic-navbar-nav" className="justify-content-center">
          <ProductSearchForm categories={categories} />
          <NavbarNav user={currentUser} />
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;

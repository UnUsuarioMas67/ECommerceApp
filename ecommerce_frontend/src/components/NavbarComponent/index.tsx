import Container from 'react-bootstrap/Container';
import Navbar from 'react-bootstrap/Navbar';
import type { Category, User } from '../../api/types';
import ProductSearchForm from './ProductSearchForm';
import NavbarNav from './NavbarNav';
import { Link, useNavigate } from '@tanstack/react-router';

type Props = {
  currentUser?: User;
  categories: Category[];
};

function NavbarComponent({ currentUser, categories }: Props) {
  const navigate = useNavigate();

  return (
    <Navbar sticky="top" expand="lg" className="bg-primary mb-4" data-bs-theme="dark">
      <Container fluid className="px-5">
        <Navbar.Brand as={Link} className="fw-bold text-white" onClick={() => navigate({ to: '/', reloadDocument: true })}>
          ECommerce
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="basic-navbar-nav" />

        <Navbar.Collapse id="basic-navbar-nav" className='justify-content-center'>
          <ProductSearchForm categories={categories} />
          <NavbarNav currentUser={currentUser} />
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;

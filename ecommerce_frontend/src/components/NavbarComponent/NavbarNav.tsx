import { useNavigate } from '@tanstack/react-router';
import { BoxArrowRight, Cart4, Gear, PersonFill } from 'react-bootstrap-icons';
import Button from 'react-bootstrap/Button';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import type { User } from '../../api/types';

type Props = {
  currentUser?: User;
};

function NavbarNav({ currentUser }: Props) {
  const navigate = useNavigate();
  const userDropdownTitle = (
    <>
      {currentUser?.firstName} {currentUser?.lastName} <PersonFill size={24} title="User dropdown" />
    </>
  );

  return (
    <Nav className="flex-shrink-0 align-items-center">
      {!currentUser ? (
        <>
          <Nav.Link onClick={() => navigate({ to: '/login' })}>Sign In</Nav.Link>
          <Button variant="outline-light" className="mx-1" onClick={() => navigate({ to: '/register' })}>
            Sign Up
          </Button>
        </>
      ) : (
        <>
          <NavDropdown title={userDropdownTitle} align="end" id="basic-nav-dropdown" data-bs-theme="light">
            <NavDropdown.Item>
              <Gear size={18} className="me-2" /> Account Settings
            </NavDropdown.Item>
            <NavDropdown.Divider />
            <NavDropdown.Item>
              <BoxArrowRight size={18} className="me-2" /> Sign Out
            </NavDropdown.Item>
          </NavDropdown>

          <Nav.Link>
            <Cart4 size={24} title="View cart" />
          </Nav.Link>
        </>
      )}
    </Nav>
  );
}

export default NavbarNav;

import { Link, useLocation, useMatchRoute, useRouter } from '@tanstack/react-router';
import { BoxArrowRight, Gear, PersonFill, GeoAlt, CartCheck } from 'react-bootstrap-icons';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import type { User } from '../../api/types';
import { useAuth } from '../AuthProvider/AuthContext';
import { postLogout } from '../../api/user';
import { useAxios } from '../../hooks/use-axios';
import ViewCartButton from './ViewCartButton';
import { useQueryClient } from '@tanstack/react-query';

type Props = {
  user: User | null;
};

function NavbarNav({ user: currentUser }: Props) {
  const { clearCredentials } = useAuth();
  const location = useLocation();
  const { invalidate: invalidateRouter } = useRouter();
  const axiosInstance = useAxios();
  const queryClient = useQueryClient();

  const onSignOutClick = async () => {
    postLogout(axiosInstance);
    clearCredentials();
    queryClient.clear();
    invalidateRouter();
  };

  const matchRoute = useMatchRoute();

  const showCartBtn = !matchRoute({ to: '/checkout', fuzzy: true });

  const userDropdownTitle = (
    <>
      {currentUser?.firstName} {currentUser?.lastName} <PersonFill size={24} title="User dropdown" />
    </>
  );

  return (
    <Nav className="flex-shrink-0 align-items-center">
      {!currentUser ? (
        <>
          <Link className="nav-link" to="/login" search={{ redirect: location.href }}>
            Sign In
          </Link>
          <Link className="mx-1 btn btn-outline-light" to="/register" search={{ redirect: location.href }}>
            Sign Up
          </Link>
        </>
      ) : (
        <>
          <NavDropdown title={userDropdownTitle} align="end" id="basic-nav-dropdown" data-bs-theme="light">
            <NavDropdown.Item as="span">
              <Link className="text-body text-decoration-none d-inline-flex align-items-center" to="/account/orders">
                <CartCheck size={18} className="me-3" />
                Your orders
              </Link>
            </NavDropdown.Item>

            <NavDropdown.Item as="span">
              <Link className="text-body text-decoration-none d-inline-flex align-items-center" to="/account/addresses">
                <GeoAlt size={18} className="me-3" />
                Your addresses
              </Link>
            </NavDropdown.Item>

            <NavDropdown.Item as="span">
              <Link className="text-body text-decoration-none d-inline-flex align-items-center" to="/account/settings">
                <Gear size={18} className="me-3" />
                Account settings
              </Link>
            </NavDropdown.Item>

            <NavDropdown.Divider />

            <NavDropdown.Item as="span">
              <Link
                className="text-body text-decoration-none d-inline-flex align-items-center"
                to="/login"
                search={{ redirect: location.href }}
                onClick={onSignOutClick}>
                <BoxArrowRight size={18} className="me-3" />
                Sign out
              </Link>
            </NavDropdown.Item>
          </NavDropdown>
        </>
      )}

      {showCartBtn && <ViewCartButton />}
    </Nav>
  );
}

export default NavbarNav;

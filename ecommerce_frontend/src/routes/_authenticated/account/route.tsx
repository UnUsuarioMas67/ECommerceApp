import { createFileRoute, Link, Outlet } from '@tanstack/react-router';
import { CartCheck, GeoAlt, Gear } from 'react-bootstrap-icons';
import Col from 'react-bootstrap/esm/Col';
import Container from 'react-bootstrap/esm/Container';
import Nav from 'react-bootstrap/esm/Nav';
import Row from 'react-bootstrap/esm/Row';

export const Route = createFileRoute('/_authenticated/account')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <Container className='flex-grow-1'>
      <Row className='h-100'>
        <Col xs={3} as="aside">
          <Nav defaultActiveKey="/home" className="flex-column" variant="underline">
            <Link className="nav-link text-decoration-none d-inline-flex align-items-center" to="/account/orders">
              <CartCheck size={18} className="me-3" />
              Your orders
            </Link>

            <Link className="nav-link text-decoration-none d-inline-flex align-items-center" to="/account/addresses">
              <GeoAlt size={18} className="me-3" />
              Your addresses
            </Link>

            <Link className="nav-link text-decoration-none d-inline-flex align-items-center" to="/account/settings">
              <Gear size={18} className="me-3" />
              Account settings
            </Link>
          </Nav>
        </Col>
        <Col as="main">
          <Outlet />
        </Col>
      </Row>
    </Container>
  );
}

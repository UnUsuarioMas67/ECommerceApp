import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';
import Navbar from 'react-bootstrap/Navbar';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import Button from 'react-bootstrap/Button';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { Row } from 'react-bootstrap';
import { Search, Cart4, PersonFill, BoxArrowRight, Gear } from 'react-bootstrap-icons';
import { Link } from '@tanstack/react-router';
import type { User } from '../types/api-types';

type Props = {
  currentUser?: User;
};

function NavbarComponent({ currentUser }: Props) {
  const userDropdownTitle = (
    <>
      Username <PersonFill size={24} title="User dropdown" />
    </>
  );

  return (
    <Navbar sticky="top" expand="lg" className="bg-primary mb-4" data-bs-theme="dark">
      <Container fluid className="px-5">
        <Navbar.Brand as={Link} to="/" className="fw-bold text-white">
          ECommerce
        </Navbar.Brand>

        <Navbar.Toggle aria-controls="basic-navbar-nav" />

        <Navbar.Collapse id="basic-navbar-nav">
          <Form style={{ maxWidth: '900px' }} className="w-100 mx-lg-4 mx-xl-auto mt-2 mt-lg-0" data-bs-theme="light">
            <Row className="gx-1 gy-2">
              <Col lg={2}>
                <Form.Select size="sm" aria-label="Filter by category">
                  <option>-- All --</option>
                  <option value="1">Category 1</option>
                  <option value="2">Category 2</option>
                  <option value="3">Category 3</option>
                </Form.Select>
              </Col>

              <Col>
                <InputGroup size="sm">
                  <Form.Control placeholder="Search..." aria-label="Search products" aria-describedby="basic-addon2" />

                  <Button variant="secondary" id="search-product-btn" type="submit">
                    Search <Search />
                  </Button>
                </InputGroup>
              </Col>
            </Row>
          </Form>

          <Nav className="flex-shrink-0 align-items-center">
            {!currentUser ? (
              <>
                <Nav.Link as={Link} to="/login">Sign In</Nav.Link>
                <Nav.Link as={Link} to="/register">
                  <Button variant="outline-light" className="m-0">
                    Sign Up
                  </Button>
                </Nav.Link>
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
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;

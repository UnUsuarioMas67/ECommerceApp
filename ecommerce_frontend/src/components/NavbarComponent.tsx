import Container from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Col';
import Navbar from 'react-bootstrap/Navbar';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import Button from 'react-bootstrap/Button';
import Nav from 'react-bootstrap/Nav';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { Row } from 'react-bootstrap';
import { Search, Cart4, PersonCircle } from 'react-bootstrap-icons';
import { Link } from '@tanstack/react-router';

function NavbarComponent() {
  const dropdownTitle = (
    <>
      <PersonCircle size={24} />
    </>
  );

  return (
    <Navbar sticky="top" expand="lg" className="bg-primary mb-4" data-bs-theme="dark">
      <Container>
        <Navbar.Brand className="fw-bold text-white">ECommerce</Navbar.Brand>

        <Navbar.Toggle aria-controls="basic-navbar-nav" />

        <Navbar.Collapse id="basic-navbar-nav">
          <Form className="w-100 mx-4 mt-2 mt-lg-0" data-bs-theme="light">
            <Row className="gx-1 gy-2">
              <Col lg={2}>
                <Form.Select size="sm" aria-label="Filter by category">
                  <option>All</option>
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

          <Nav>
            <Nav.Link>
              <Cart4 size={24} />
            </Nav.Link>
            <NavDropdown title={dropdownTitle} id="basic-nav-dropdown" data-bs-theme="light">
              <NavDropdown.Item as={Link} to='/login'>Sign In</NavDropdown.Item>
              <NavDropdown.Divider />
              <NavDropdown.Header>
                Don't have an account? <br />
                <Link to="/register">Register here</Link>
              </NavDropdown.Header>
            </NavDropdown>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;

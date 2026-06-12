import Container from 'react-bootstrap/Container';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import Form from 'react-bootstrap/Form';
import InputGroup from 'react-bootstrap/InputGroup';
import Button from 'react-bootstrap/Button';

function NavbarComponent() {
  return (
    <Navbar expand="lg" className="bg-dark mb-4 py-3" data-bs-theme="dark">
      <Container>
        <Navbar.Brand className='fw-bold'>ECommerce</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            <Nav.Link href="#home">Home</Nav.Link>
            <Nav.Link href="#link">Link</Nav.Link>
          </Nav>
          <Form>
            <InputGroup>
              <Form.Control
                placeholder="Search..."
                aria-label="Search"
                aria-describedby="basic-addon2"
              />
              <Button variant="primary" id="button-addon2" type='submit'>
                Search
              </Button>
            </InputGroup>
          </Form>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default NavbarComponent;

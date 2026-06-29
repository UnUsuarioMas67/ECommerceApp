import { createFileRoute, Link } from '@tanstack/react-router';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import Button from 'react-bootstrap/esm/Button';
import { ArrowLeft, CaretLeft, CaretLeftFill, ChevronLeft } from 'react-bootstrap-icons';
import RegisterForm from './-register-form';

export const Route = createFileRoute('/(auth)/register')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <Row as='main' className="flex-grow-1 justify-content-center align-items-center p-3 p-md-0">
      <Col md={6} xl={4}>
        <Button as={Link} to="/login" variant="outline-secondary" className='border-0 rounded-circle mb-2 p-2' size='sm'>
          <ChevronLeft size={24} />
        </Button>
        <h1 className="text-center text-body mb-5">Create a new account</h1>
        <RegisterForm />
      </Col>
    </Row>
  );
}

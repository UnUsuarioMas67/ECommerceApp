import { createFileRoute } from '@tanstack/react-router';
import { CheckCircleFill } from 'react-bootstrap-icons';
import Col from 'react-bootstrap/esm/Col';
import Container from 'react-bootstrap/esm/Container';
import Row from 'react-bootstrap/esm/Row';

export const Route = createFileRoute('/_authenticated/checkout/success')({
  component: RouteComponent,
});

function RouteComponent() {
  return (
    <Container as="main">
      <Row>
        <Col>
          <div className="text-center mb-3">
            <div className="text-success mb-3">
              <CheckCircleFill size={172} />
            </div>
            <h3 className="text-body">Payment successfull</h3>
          </div>
        </Col>
      </Row>
    </Container>
  );
}

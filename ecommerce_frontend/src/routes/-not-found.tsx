import Col from "react-bootstrap/esm/Col";
import Row from "react-bootstrap/esm/Row";

function NotFoundPage() {
  return (
    <Row className="pt-5 justify-content-center">
      <Col className="text-center">
        <p className='text-primary display-2 fw-bold'>Not Found</p>
        <p className='fs-5 text-body-secondary'>Could not find the page you we're looking for.</p>
      </Col>
    </Row>
  );
}

export default NotFoundPage;
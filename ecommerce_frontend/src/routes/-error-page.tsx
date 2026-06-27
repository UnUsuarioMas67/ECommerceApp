import Col from "react-bootstrap/esm/Col";
import Row from "react-bootstrap/esm/Row";

function ErrorPage() {
  return (
    <Row className="pt-5 justify-content-center">
      <Col className="text-center">
        <p className='text-primary display-2 fw-bold'>Oops!</p>
        <p className='fs-5 text-body-secondary'>An unexpected error has ocurred. Please try again later.</p>
      </Col>
    </Row>
  );
}

export default ErrorPage;
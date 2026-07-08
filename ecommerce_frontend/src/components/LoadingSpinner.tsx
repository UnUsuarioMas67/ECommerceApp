import Row from 'react-bootstrap/esm/Row';
import Spinner from 'react-bootstrap/Spinner';

function LoadingSpinner() {
  return (
    <Row className="justify-content-center mt-5">
      <Spinner animation="border" role="status" variant="primary">
        <span className="visually-hidden">Loading...</span>
      </Spinner>
    </Row>
  );
}

export default LoadingSpinner;

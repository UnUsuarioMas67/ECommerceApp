import Offcanvas from 'react-bootstrap/esm/Offcanvas';

type Props = {
  show: boolean;
  onHide: () => void;
};

function ShoppingCartSidebar({ show, onHide }: Props) {
  return (
    <Offcanvas show={show} onHide={onHide} placement="end" scroll>
      <Offcanvas.Header closeButton>
        <Offcanvas.Title>Shopping cart</Offcanvas.Title>
      </Offcanvas.Header>
      <Offcanvas.Body>
        <span className="text-body-tertiary">No items yet.</span>
      </Offcanvas.Body>
    </Offcanvas>
  );
}

export default ShoppingCartSidebar;

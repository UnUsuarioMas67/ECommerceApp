import Card from 'react-bootstrap/esm/Card';
import type { Address } from '../api/types';
import Button from 'react-bootstrap/esm/Button';
import { PencilFill, TrashFill } from 'react-bootstrap-icons';

type Props = {
  address: Address;
  onEditClick?: () => void
  onDeleteClick?: () => void
};

function AddressCard({ address, onEditClick, onDeleteClick }: Props) {
  return (
    <Card>
      <Card.Body>
        <span>
          {address.addressLine1}{address.addressLine2 && `, ${address.addressLine2}`}.
        </span>
        <br />
        <span>
          {address.city}, {address.region}, {address.country}, {address.postalCode}.
        </span>
      </Card.Body>
      <Card.Footer>
        <Button variant="secondary" size="sm" className="me-2 d-inline-flex align-items-center gap-1" onClick={onEditClick}>
          <PencilFill /> Edit
        </Button>
        <Button variant="danger" size="sm" className="d-inline-flex align-items-center gap-1" onClick={onDeleteClick}>
          <TrashFill /> Delete
        </Button>
      </Card.Footer>
    </Card>
  );
}

export default AddressCard;

import { createFileRoute } from '@tanstack/react-router';
import { fetchAddresses, fetchCountries } from '../../../api/addresses';
import { useSuspenseQuery } from '@tanstack/react-query';
import { useAxios } from '../../../hooks/use-axios';
import { PlusLg } from 'react-bootstrap-icons';
import AddressCard from '../../../components/AddressCard';
import Row from 'react-bootstrap/esm/Row';
import Col from 'react-bootstrap/esm/Col';
import Button from 'react-bootstrap/esm/Button';
import { useState } from 'react';
import CreateAddressModal from '../../../components/Addresses/CreateAddressModal';
import Alert from 'react-bootstrap/esm/Alert';
import DeleteAddressModal from '../../../components/Addresses/DeleteAddressModal';
import UpdateAddressModal from '../../../components/Addresses/UpdateAddressModal';
import Container from 'react-bootstrap/esm/Container';
import LoadingSpinner from '../../../components/LoadingSpinner';

export const Route = createFileRoute('/_authenticated/account/addresses')({
  component: RouteComponent,
  pendingComponent: LoadingSpinner,
  loader: ({ context: { axiosInstance, queryClient } }) => {
    queryClient.ensureQueryData({
      queryKey: ['countries'],
      queryFn: () => fetchCountries(axiosInstance),
      staleTime: Infinity,
    });
    queryClient.prefetchQuery({
      queryKey: ['addresses'],
      queryFn: () => fetchAddresses(axiosInstance),
    });
  },
});

type AddressesPageModalType = 'add' | 'update' | 'delete';
type AddressesPageModalState = AddressesPageModalType | false;
type AddressesModalOptions = { modal: 'add' } | { modal: 'update' | 'delete'; id: number };

function RouteComponent() {
  const axiosInstance = useAxios();
  const { data: countries } = useSuspenseQuery({
    queryKey: ['countries'],
    queryFn: () => fetchCountries(axiosInstance),
    staleTime: Infinity,
  });
  const { data: addresses } = useSuspenseQuery({
    queryKey: ['addresses'],
    queryFn: () => fetchAddresses(axiosInstance),
  });

  const [modal, setModal] = useState<AddressesPageModalState>(false);
  const [updatedId, setUpdatedId] = useState<number | undefined>(undefined);
  const [deletedId, setDeletedId] = useState<number | undefined>(undefined);

  const openModal = (options: AddressesModalOptions) => {
    if (options.modal === 'update') setUpdatedId(options.id);
    else if (options.modal === 'delete') setDeletedId(options.id);

    setModal(options.modal);
  };
  const closeModal = () => {
    setModal(false);
  };

  const [error, setError] = useState<string | null>(null);

  return (
    <Container>
      <h1 className="mb-4">Adresses</h1>

      {error && (
        <Alert variant="danger" onClose={() => setError(null)} dismissible>
          {error}
        </Alert>
      )}

      <Row className="g-3" xs={1} md={2} lg={3}>
        <Col>
          <Button
            variant="secondary"
            className="d-inline-block w-100 h-100"
            size="lg"
            onClick={() => openModal({ modal: 'add' })}>
            <PlusLg size={32} />
            <br />
            <span className="fs-5">Add address</span>
          </Button>
        </Col>

        {addresses.map((address) => (
          <Col key={address.id}>
            <AddressCard
              address={address}
              onDeleteClick={() => openModal({ modal: 'delete', id: address.id })}
              onEditClick={() => openModal({ modal: 'update', id: address.id })}
            />
          </Col>
        ))}
      </Row>

      <CreateAddressModal show={modal === 'add'} onHide={closeModal} countries={countries} />
      {updatedId && (
        <UpdateAddressModal addressId={updatedId} countries={countries} show={modal === 'update'} onHide={closeModal} />
      )}
      {deletedId && (
        <DeleteAddressModal
          addressId={deletedId}
          show={modal === 'delete'}
          onHide={closeModal}
          onError={() => setError('Could not delete the address due to an error')}
        />
      )}
    </Container>
  );
}

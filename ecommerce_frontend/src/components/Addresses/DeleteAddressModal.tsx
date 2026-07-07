import React from 'react';
import Button from 'react-bootstrap/esm/Button';
import Modal from 'react-bootstrap/esm/Modal';
import { useAxios } from '../../hooks/use-axios';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { deleteAddress } from '../../api/addresses';

type Props = {
  addressId: number;
  show: boolean;
  onHide?: () => void;
  onError?: (error: Error) => void
};

function DeleteAddressModal({ addressId, show, onHide, onError }: Props) {
  const queryClient = useQueryClient();
  const axiosInstance = useAxios();
  const { mutate } = useMutation<void, Error, number>({
    mutationFn: (id) => deleteAddress(axiosInstance, id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['addresses'] });
      onHide?.();
    },
    onError: (error) => {
      onError?.(error)
      onHide?.();
    }
  });

  return (
    <Modal show={show} onHide={onHide}>
      <Modal.Header closeButton>
        <Modal.Title>Delete an address</Modal.Title>
      </Modal.Header>
      <Modal.Body>Are you sure you want to delete this address?</Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          Cancel
        </Button>
        <Button variant="danger" onClick={() => mutate(addressId)}>
          Ok
        </Button>
      </Modal.Footer>
    </Modal>
  );
}

export default DeleteAddressModal;

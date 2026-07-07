import { zodResolver } from '@hookform/resolvers/zod';
import Button from 'react-bootstrap/esm/Button';
import Modal from 'react-bootstrap/esm/Modal';
import Form from 'react-bootstrap/esm/Form';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { useAxios } from '../../hooks/use-axios';
import { addressUpdateSchema, type AddressUpdate } from '../../schemas/addresses';
import FloatingLabel from 'react-bootstrap/esm/FloatingLabel';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { fetchAddress, updateAddress } from '../../api/addresses';
import { isAxiosError } from 'axios';
import type { Api422ErrorBody, Country } from '../../api/types';
import { useEffect } from 'react';
import Alert from 'react-bootstrap/esm/Alert';
import LoadingSpinner from '../LoadingSpinner';

type Props = {
  countries: Country[];
  addressId: number;
  show: boolean;
  onHide?: () => void;
};

function handleErrorMessage(error: Error): string | Api422ErrorBody {
  if (isAxiosError(error)) {
    if (error.response) {
      if (error.response.status === 422) return error.response.data as Api422ErrorBody;

      if (error.response.status >= 500) return 'Server error. Please try again later.';

      return 'Registration failed due to an unexpected error.';
    }

    return 'No response from server.';
  }

  throw error;
}

function UpdateAddressModal({ show, onHide, countries, addressId }: Props) {
  const axiosInstance = useAxios();

  const { data: address, isPending: queryPending } = useQuery({
    queryKey: ['addresses', addressId],
    queryFn: () => fetchAddress(axiosInstance, addressId),
  });

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
    reset,
  } = useForm<AddressUpdate>({
    resolver: zodResolver(addressUpdateSchema),
  });

  const queryClient = useQueryClient();
  const { mutate, isPending: mutationPending } = useMutation<void, Error, AddressUpdate>({
    mutationFn: (data) => updateAddress(axiosInstance, addressId, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['addresses'] });
    },

    onError: (error) => {
      const errorObj = handleErrorMessage(error);
      if (typeof errorObj !== 'string') {
        if (errorObj.errorType === 'invalid_country_code') {
          setError('countryCode', { message: 'Invalid country' });
        } else {
          throw error;
        }
      } else {
        setError('root', { message: errorObj });
      }
    },
  });

  const onSubmit: SubmitHandler<AddressUpdate> = (data) => {
    mutate(data);
    onHide?.();
  };

  useEffect(() => {
    if (show && !queryPending) reset(address);
  }, [show, reset, queryPending, address]);

  return (
    <Modal show={show} onHide={onHide} backdrop="static">
      <Modal.Header closeButton>
        <Modal.Title>Update an address</Modal.Title>
      </Modal.Header>

      {queryPending ? (
        <LoadingSpinner />
      ) : (
        <>
          <Modal.Body>
            {errors.root && <Alert variant="danger">{errors.root.message}</Alert>}

            <Form onSubmit={handleSubmit(onSubmit)} id="update-address-form">
              <FloatingLabel controlId="floatingAl1" label="Address line 1" className="mb-3">
                <Form.Control
                  placeholder="Address line 1"
                  {...register('addressLine1')}
                  isInvalid={!!errors.addressLine1}
                />
                <Form.Control.Feedback type="invalid">{errors.addressLine1?.message}</Form.Control.Feedback>
              </FloatingLabel>

              <FloatingLabel controlId="floatingAl2" label="Address line 2" className="mb-3">
                <Form.Control
                  placeholder="Address line 2"
                  {...register('addressLine2')}
                  isInvalid={!!errors.addressLine2}
                />
                <Form.Control.Feedback type="invalid">{errors.addressLine2?.message}</Form.Control.Feedback>
              </FloatingLabel>

              <FloatingLabel controlId="floatingCity" label="City" className="mb-3">
                <Form.Control placeholder="City" {...register('city')} isInvalid={!!errors.city} />
                <Form.Control.Feedback type="invalid">{errors.city?.message}</Form.Control.Feedback>
              </FloatingLabel>

              <FloatingLabel controlId="floatingRegion" label="Region" className="mb-3">
                <Form.Control placeholder="Region" {...register('region')} isInvalid={!!errors.region} />
                <Form.Control.Feedback type="invalid">{errors.region?.message}</Form.Control.Feedback>
              </FloatingLabel>

              <FloatingLabel controlId="floatingCountrySelect" label="Country" className="mb-3">
                <Form.Select aria-label="Country select" {...register('countryCode')} isInvalid={!!errors.countryCode}>
                  <option>-- Select a country --</option>
                  {countries.map((country) => (
                    <option key={country.cca2} value={country.cca2}>
                      ({country.cca2.toUpperCase()}) {country.name}
                    </option>
                  ))}
                </Form.Select>
                <Form.Control.Feedback type="invalid">{errors.countryCode?.message}</Form.Control.Feedback>
              </FloatingLabel>

              <FloatingLabel controlId="floatingZip" label="Postal code" className="mb-3">
                <Form.Control placeholder="Postal code" {...register('postalCode')} isInvalid={!!errors.postalCode} />
                <Form.Control.Feedback type="invalid">{errors.postalCode?.message}</Form.Control.Feedback>
              </FloatingLabel>
            </Form>
          </Modal.Body>

          <Modal.Footer>
            <Button variant="secondary" onClick={onHide}>
              Close
            </Button>

            <Button variant="primary" type="submit" form="update-address-form" disabled={mutationPending}>
              {mutationPending || isSubmitting ? 'Updating...' : 'Update'}
            </Button>
          </Modal.Footer>
        </>
      )}
    </Modal>
  );
}

export default UpdateAddressModal;

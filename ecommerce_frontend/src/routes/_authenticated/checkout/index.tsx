import { createFileRoute, redirect } from '@tanstack/react-router';
import Container from 'react-bootstrap/esm/Container';
import Form from 'react-bootstrap/esm/Form';
import {
  mutationOptions,
  queryOptions,
  useMutation,
  useSuspenseQueries,
  useSuspenseQuery,
} from '@tanstack/react-query';
import { fetchProduct } from '../../../api/products';
import { useCart } from '../../../components/CartProvider/CartContext';
import { useAxios } from '../../../hooks/use-axios';
import { fetchAddresses, fetchCountries } from '../../../api/addresses';
import Row from 'react-bootstrap/esm/Row';
import Button from 'react-bootstrap/esm/Button';
import CreateAddressModal from '../../../components/Addresses/CreateAddressModal';
import { useState } from 'react';
import type { AxiosInstance } from 'axios';
import { checkoutSchema, type CartItemRequest, type CheckoutRequest } from '../../../schemas/checkout';
import Col from 'react-bootstrap/esm/Col';
import type { CheckoutSession } from '../../../api/types';
import {
  createCheckoutSession,
  isProductsStockError,
  ProductsStockError,
  type ProductsStockErrorItem,
} from '../../../api/checkout';
import Spinner from 'react-bootstrap/esm/Spinner';
import ProductCard from '../../../components/ProductCard';
import { useCheckoutError } from '../../../components/CheckoutErrorProvider/CheckoutErrorContext';

const productsQuery = (axiosInstance: AxiosInstance, item: CartItemRequest) =>
  queryOptions({
    queryKey: ['products', item.productId],
    queryFn: () => fetchProduct(axiosInstance, item.productId),
    staleTime: 1000 * 60 * 5, // 5 mins
    refetchOnWindowFocus: false,
  });

const addressesQuery = (axiosInstance: AxiosInstance) =>
  queryOptions({
    queryKey: ['addresses'],
    queryFn: () => fetchAddresses(axiosInstance),
  });

const countriesQuery = (axiosInstance: AxiosInstance) =>
  queryOptions({
    queryKey: ['countries'],
    queryFn: () => fetchCountries(axiosInstance),
    staleTime: Infinity,
  });

const checkoutMutation = (axiosInstance: AxiosInstance) =>
  mutationOptions<CheckoutSession, Error, CheckoutRequest>({
    mutationFn: (data) => createCheckoutSession(axiosInstance, data),
  });

export const Route = createFileRoute('/_authenticated/checkout/')({
  component: RouteComponent,
  beforeLoad: ({ context: { cartContext } }) => {
    if (cartContext.items.length === 0) throw redirect({ to: '/' });
  },
  loader: async ({ context: { cartContext, queryClient, axiosInstance } }) => {
    const { items } = cartContext;

    await Promise.all([
      ...items.map((item) => queryClient.prefetchQuery(productsQuery(axiosInstance, item))),
      queryClient.ensureQueryData(countriesQuery(axiosInstance)),
      queryClient.prefetchQuery(addressesQuery(axiosInstance)),
    ]);
  },
});

function RouteComponent() {
  const { items, getItemQuantity } = useCart();
  const navigate = Route.useNavigate();
  const [showModal, setShowModal] = useState(false);
  const { setError, setProducts: setErrorProducts } = useCheckoutError();

  const axiosInstance = useAxios();

  const { data: countries } = useSuspenseQuery(countriesQuery(axiosInstance));
  const { data: addresses } = useSuspenseQuery(addressesQuery(axiosInstance));
  const { products } = useSuspenseQueries({
    queries: items.map((item) => productsQuery(axiosInstance, item)),
    combine: (result) => {
      return {
        products: result.filter((query) => !query.isError).map((query) => query.data!),
        isPending: result.some((query) => query.isPending),
      };
    },
  });

  const totalCost = products
    .map((product) => product.price * getItemQuantity(product.id))
    .reduce((prev, curr) => prev + curr, 0)
    .toFixed(2);

  const { mutate, isPending: mutationPending } = useMutation({
    ...checkoutMutation(axiosInstance),
    onSuccess: (result) => {
      navigate({ href: result.url });
    },
    onError: (error) => {
      if (isProductsStockError(error)) setErrorProducts(error.products);

      setError('An error has ocurred while processing the order.');
      navigate({ to: 'error' });
    },
  });

  const [selectedAddress, setSelectedAddress] = useState<number | undefined>(
    addresses && addresses.length > 0 ? addresses[0].id : undefined,
  );

  const [btnPressed, setBtnPressed] = useState(false);

  const onPayBtnClick = () => {
    if (!selectedAddress) return;

    const data: CheckoutRequest = { addressId: selectedAddress, cart: { items } };
    const schema = checkoutSchema.parse(data);

    setBtnPressed(true);
    mutate(schema);
  };

  return (
    <Container className="px-md-5" as="main">
      <Row className="mb-4">
        <Col>
          <h3>Delivery location</h3>
          <Form.Group className="mb-3">
            <Form.Label className="h5">Select an address</Form.Label>
            <Form.Select onChange={(e) => setSelectedAddress(Number(e.target.value))} defaultValue={selectedAddress}>
              {addresses.map((address) => (
                <option key={address.id} value={address.id}>
                  {`${address.addressLine1} ${address.addressLine2 ? ', ' + address.addressLine2 : ''}, ${address.city}, ${address.region}, ${address.country}, ${address.postalCode} `}
                </option>
              ))}
            </Form.Select>
          </Form.Group>
          <Button onClick={() => setShowModal(true)}>Add an address</Button>
        </Col>
      </Row>

      <Row className="mb-4">
        <Col>
          <h3>Products overview</h3>
          <Row xs={1} className="g-3">
            {products.map((product) => (
              <Col>
                <ProductCard product={product} key={product.id} subtotal disableLink quantityOnly />
              </Col>
            ))}
          </Row>
          <hr />
          <div className="px-3 d-flex justify-content-between">
            <p className="fs-5 fw-bold">Total:</p>
            <p className="fs-5">$ {totalCost}</p>
          </div>
        </Col>
      </Row>

      <Row className="mb-5">
        <Col>
          <Button
            size="lg"
            variant="success"
            disabled={products.length === 0 || !selectedAddress || mutationPending || btnPressed}
            onClick={onPayBtnClick}>
            {mutationPending ? (
              <>
                <Spinner size="sm" /> Processing payment
              </>
            ) : (
              'Pay with Stripe'
            )}
          </Button>
        </Col>
      </Row>

      <CreateAddressModal countries={countries} show={showModal} onHide={() => setShowModal(false)} />
    </Container>
  );
}

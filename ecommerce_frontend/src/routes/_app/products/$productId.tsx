import { createFileRoute } from '@tanstack/react-router';
import type { AxiosInstance } from 'axios';
import { useAxios } from '../../../hooks/use-axios';
import type { Product } from '../../../api/types';
import { useQuery } from '@tanstack/react-query';
import ProductCard from '../../../components/ProductCard';
import { imagesUrl } from '../../../api';

export const Route = createFileRoute('/_app/products/$productId')({
  component: RouteComponent,
});

async function fetchProduct(id: string, axios: AxiosInstance) {
  const response = await axios.get<Product>(`/products/${id}`);
  return response.data;
}

function RouteComponent() {
  const axiosInstance = useAxios();
  const { productId } = Route.useParams();
  const { data, isLoading, isError } = useQuery({
    queryKey: ['products', productId],
    queryFn: () => fetchProduct(productId, axiosInstance),
  });

  if (isLoading) return <h1>Loading</h1>;

  if (isError) return <h1 className="text-danger">Error</h1>;

  return data && <ProductCard imagesUrl={imagesUrl} product={data} />;
}

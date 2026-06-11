import { useEffect, type ReactNode } from 'react';
import { useNavigate } from 'react-router';
import { useAxios } from '../../hooks/use-axios';
import { useQuery } from '@tanstack/react-query';
import type { User } from '../../types/api-types';

type Props = {
  children: ReactNode;
};

function ProtectedRoute({ children }: Props) {
  const navigate = useNavigate();
  const axiosInstance = useAxios();
  const { isLoading, isError } = useQuery({
    queryKey: ['currentUser'],
    queryFn: async () => {
      const response = await axiosInstance.get<User>('/clients/me');
      return response.data;
    },
    staleTime: Infinity,
    gcTime: Infinity,
    retry: 2,
  });

  useEffect(() => {
    if (isError) navigate('/login');
  }, [isError, navigate]);

  if (isLoading) {
    return <p>Please wait...</p>;
  }

  if (isError) {
    return <p className="text-danger">Error</p>;
  }

  return children;
}

export default ProtectedRoute;

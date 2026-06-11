import { useMutation } from '@tanstack/react-query';
import { useAxios } from './use-axios';
import type { UserAuth } from '../types/api-types';
import { useAuth } from '../components/auth/context/AuthContext';

type LoginRequest = { email: string; password: string };

export function useLogin() {
  const axiosInstance = useAxios();
  const { setCredentials } = useAuth();

  return useMutation<UserAuth, Error, LoginRequest>({
    mutationFn: async (data) => {
      const response = await axiosInstance.post<UserAuth>('/clients/login', data);
      return response.data;
    },
    onSuccess: (data) => {
      setCredentials(data);
    },
  });
}

export function useLogout() {
  const axiosInstance = useAxios();
  const { clearCredentials } = useAuth();

  return useMutation<undefined, Error, undefined>({
    mutationFn: async () => {
      await axiosInstance.post('/clients/logout');
    },
    onSuccess: () => {
      clearCredentials();
    },
  });
}

// register hook here

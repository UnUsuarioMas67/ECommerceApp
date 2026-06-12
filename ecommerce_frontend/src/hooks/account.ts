import { useMutation } from '@tanstack/react-query';
import { useAxios } from './use-axios';
import { useAuth } from '../components/auth/context/AuthContext';
import type { UserAuth } from '../types/api-types';
import type { LoginRequest } from '../schemas/account';

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

import { useMutation, useQuery } from '@tanstack/react-query';
import { useAxios } from './use-axios';
import { useAuth } from '../components/auth/context/AuthContext';
import type { User, UserAuth } from '../types/api-types';
import type { LoginRequest, RegisterRequest } from '../schemas/account';

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

export function useRegister() {
  const axiosInstance = useAxios();

  return useMutation<User, Error, RegisterRequest>({
    mutationFn: async (data) => {
      const response = await axiosInstance.post<User>('/clients/login', data);
      return response.data;
    },
  });
}   

export function useCurrrentUser() {
  const axiosInstance = useAxios();
  
  return useQuery({
    queryKey: ['currentUser'],
    queryFn: async () => {
      const response = await axiosInstance.get<User>('/clients/me');
      return response.data;
    },
    staleTime: Infinity,
    retry: 2,
  });
}
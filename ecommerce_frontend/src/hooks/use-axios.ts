import axios, { type InternalAxiosRequestConfig } from 'axios';
import { useAuth } from '../components/auth/context/AuthContext';
import { useMemo } from 'react';

export type ApiRequestError = {
  type: 'failure_response' | 'request_failed' | 'not_found' | 'unauthorized' | 'server_error' | 'aborted';
  message?: string;
};

type ExternalAxiosRequestConfig = InternalAxiosRequestConfig & { _retry?: boolean };

export function useAxios() {
  const { getAccessToken } = useAuth();

  const axiosInstance = useMemo(() => {
    const instance = axios.create({
      baseURL: 'http://localhost:5113/api',
    });

    instance.interceptors.request.use(async (config: ExternalAxiosRequestConfig) => {
      console.log('request interceptor in use');
      const accessToken = await getAccessToken();
      if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
      }
      return config;
    });

    return instance;
  }, [getAccessToken]);

  return axiosInstance;
}

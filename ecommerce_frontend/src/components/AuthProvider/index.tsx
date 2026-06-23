import { useState, useCallback, useRef } from 'react';
import { AuthContext } from './AuthContext';
import axios from 'axios';
import type { UserAuth, User } from '../../../api/types';

type AuthState = {
  token: string;
  user: User;
  expiresAt: number;
};

const axiosInstance = axios.create({ baseURL: 'http://localhost:5113/api' });

function AuthProvider({ children }: { children: React.ReactNode }) {
  const [auth, setAuth] = useState<AuthState | null>(null);
  const abortControllerRef = useRef<AbortController | null>(null);
  const refreshPromiseRef = useRef<Promise<UserAuth | null>>(null);

  const setRefreshToken = useCallback((rt: string) => {
    localStorage.setItem('refreshToken', rt);
  }, []);

  const getRefreshToken = useCallback(() => {
    return localStorage.getItem('refreshToken');
  }, []);

  const setCredentials = useCallback(
    (auth: UserAuth) => {
      setRefreshToken(auth.refreshToken);
      setAuth({ token: auth.accessToken, user: auth.user, expiresAt: Date.parse(auth.expiresAt) });
    },
    [setRefreshToken],
  );

  const clearCredentials = useCallback(() => {
    setAuth(null);
    localStorage.removeItem('refreshToken');
  }, []);

  const fetchRefresh = useCallback(async (rt: string) => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort('New refresh request sended.');
    }

    abortControllerRef.current = new AbortController();

    try {
      const response = await axiosInstance.post<UserAuth>(
        '/clients/refresh',
        { refreshToken: rt },
        { signal: abortControllerRef.current.signal },
      );

      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        console.log('Refresh request failed. Invalid token.');
        return null;
      } else {
        throw error;
      }
    } finally {
      abortControllerRef.current = null;
    }
  }, []);

  const getAccessToken = useCallback(async () => {
    if (refreshPromiseRef.current) {
      const newAuth = await refreshPromiseRef.current;
      return newAuth?.accessToken ?? null;
    }

    const isTokenExpired = () => {
      if (!auth?.expiresAt) return true;

      return auth?.expiresAt < Date.now();
    };

    if (auth && !isTokenExpired()) return auth.token;

    const refreshToken = getRefreshToken();

    if (!refreshToken) {
      clearCredentials();
      console.error('No refresh token found');
      return null;
    }

    refreshPromiseRef.current = fetchRefresh(refreshToken).finally(() => (refreshPromiseRef.current = null));

    const newAuth = await refreshPromiseRef.current;
    if (newAuth) {
      setCredentials(newAuth);
      console.log('Refresh attempt successful');
      return newAuth.accessToken;
    }

    console.error('Invalid refresh token');
    clearCredentials();
    return null;
  }, [auth, clearCredentials, fetchRefresh, getRefreshToken, setCredentials]);

  return (
    <AuthContext.Provider
      value={{
        getAccessToken,
        setCredentials,
        clearCredentials,
        ensureLoggedIn: async () => !!(await getAccessToken()),
      }}>
      {children}
    </AuthContext.Provider>
  );
}

export default AuthProvider;

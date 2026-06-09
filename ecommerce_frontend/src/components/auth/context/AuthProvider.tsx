import { useState, useRef, useCallback } from 'react';
import { AuthContext } from './AuthContext';
import axios from 'axios';

import type { UserAuth, User } from '../../../types/api-types';

type AuthState = {
  token: string;
  user: User;
  expiresAt: number;
};

function AuthProvider({ children }: { children: React.ReactNode }) {
  const [authState, setAuthState] = useState<AuthState | null>(null);
  const [refreshToken, setRefreshToken] = useState<string | null>(sessionStorage.getItem('refreshToken'));

  const refreshPromiseRef = useRef<Promise<UserAuth | null> | null>(null);
  const abortControllerRef = useRef<AbortController | null>(null);

  const logout = useCallback(() => {
    sessionStorage.removeItem('refreshToken');
    if (refreshToken) setRefreshToken(null);
    if (authState) setAuthState(null);
  }, [authState, refreshToken]);

  const login = (userTokens: UserAuth) => {
    setAuthState({
      token: userTokens.accessToken,
      user: userTokens.user,
      expiresAt: Date.parse(userTokens.expiresAt),
    });
    setRefreshToken(userTokens.refreshToken);
    sessionStorage.setItem('refreshToken', userTokens.refreshToken);
  };

  const performRefresh = useCallback(async (rt: string): Promise<UserAuth | null> => {
    if (abortControllerRef.current) {
      abortControllerRef.current.abort();
    }

    abortControllerRef.current = new AbortController();

    try {
      const response = await axios.post<UserAuth>(
        'http://localhost:5113/api/clients/refresh',
        { refreshToken: rt },
        {
          signal: abortControllerRef.current.signal,
          validateStatus: (status) => {
            const isSuccess = status >= 200 || status < 300;
            const isUnauthorized = status === 401;
            return isSuccess || isUnauthorized;
          },
        },
      );

      // If the refresh token is invalid or expired, the server will respond with 401 Unauthorized
      if (response.status === 401) {
        console.log('Invalid or expired refresh token');
        return null;
      }

      const userTokens = response.data;

      console.log(userTokens);
      return userTokens;
    } catch (error) {
      if (error instanceof Error && error.name === 'AbortError') {
        console.log('Refresh cancelled');
      }

      throw error;
    }
  }, []);

  const getValidToken = useCallback(async (): Promise<string | null> => {
    if (refreshPromiseRef.current) {
      const userTokens = await refreshPromiseRef.current;
      return userTokens?.accessToken ?? null;
    }

    const isTokenExpired = () => {
      if (!authState?.expiresAt) return true;
      return authState.expiresAt >= Date.now();
    };

    if (authState && !isTokenExpired()) return authState.token;

    if (!refreshToken) {
      logout();
      return null;
    }

    refreshPromiseRef.current = performRefresh(refreshToken).finally(() => {
      refreshPromiseRef.current = null;
    });

    const userTokens = await refreshPromiseRef.current;
    if (!userTokens) {
      logout();
      return null;
    }

    login(userTokens);
    return userTokens.accessToken;
  }, [authState, refreshToken, performRefresh, logout]);

  return (
    <AuthContext
      value={{
        login: login,
        logout: logout,
        getValidToken,
        getUser: () => authState?.user ?? null,
        isAuthenticated: () => !!getValidToken(),
      }}>
      {children}
    </AuthContext>
  );
}

export default AuthProvider;

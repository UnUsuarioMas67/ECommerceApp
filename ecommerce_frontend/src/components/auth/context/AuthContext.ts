import { createContext, useContext } from 'react';
import type { UserAuth } from '../../../types/api-types';

type AuthContextType = {
  getAccessToken: () => Promise<string | null>;
  setCredentials: (auth: UserAuth) => void;
  clearCredentials: () => void;
  ensureLoggedIn: () => Promise<boolean>;
};

export const AuthContext = createContext<AuthContextType | null>(null);

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within an AuthProvider');

  return context;
}

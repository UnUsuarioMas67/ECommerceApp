import { createContext, useContext } from 'react';
import type { UserAuth, User } from '../../../types/api-types';

type AuthContextType = {
  login: (auth: UserAuth) => void;
  logout: () => void;
  getValidToken: () => Promise<string | null>;
  getUser: () => User | null;
  isAuthenticated: () => boolean;
};

export const AuthContext = createContext<AuthContextType | null>(null);

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within an AuthProvider');

  return context;
}

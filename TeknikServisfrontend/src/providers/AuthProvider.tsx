'use client';

import { useEffect, useState, createContext, useContext, ReactNode } from 'react';
import { useAuthStore, useAuthHydration } from '@/store/authStore';

interface AuthContextType {
  isHydrated: boolean;
  isAuthenticated: boolean;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType>({
  isHydrated: false,
  isAuthenticated: false,
  isLoading: true,
});

export const useAuth = () => useContext(AuthContext);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [mounted, setMounted] = useState(false);
  const hasHydrated = useAuthHydration();
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const token = useAuthStore((state) => state.token);

  useEffect(() => {
    setMounted(true);
    console.log('[AuthProvider] Mounted');

    // Debug: Check localStorage directly
    if (typeof window !== 'undefined') {
      const stored = localStorage.getItem('auth-storage');
      console.log('[AuthProvider] localStorage auth-storage:', stored);
    }
  }, []);

  useEffect(() => {
    console.log('[AuthProvider] State changed:', {
      mounted,
      hasHydrated,
      isAuthenticated,
      hasToken: !!token
    });
  }, [mounted, hasHydrated, isAuthenticated, token]);

  const isHydrated = mounted && hasHydrated;

  return (
    <AuthContext.Provider
      value={{
        isHydrated,
        isAuthenticated,
        isLoading: !isHydrated,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

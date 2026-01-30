'use client';

import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import { Kullanici } from '@/types';

interface AuthState {
  token: string | null;
  refreshToken: string | null;
  user: Kullanici | null;
  isAuthenticated: boolean;
  _hasHydrated: boolean;
  setAuth: (token: string, refreshToken: string, user: Kullanici) => void;
  logout: () => void;
  updateUser: (user: Kullanici) => void;
  setHasHydrated: (state: boolean) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      refreshToken: null,
      user: null,
      isAuthenticated: false,
      _hasHydrated: false,

      setAuth: (token, refreshToken, user) => {
        console.log('[AuthStore] setAuth called with:', { token: token?.substring(0, 20) + '...', user: user?.email });
        set({
          token,
          refreshToken,
          user,
          isAuthenticated: true,
        });
        // Force sync to localStorage
        const state = get();
        console.log('[AuthStore] State after setAuth:', { isAuthenticated: state.isAuthenticated, hasToken: !!state.token });
      },

      logout: () => {
        console.log('[AuthStore] logout called');
        set({
          token: null,
          refreshToken: null,
          user: null,
          isAuthenticated: false,
        });
      },

      updateUser: (user) => set({ user }),

      setHasHydrated: (state) => {
        set({ _hasHydrated: state });
      },
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => {
        // SSR check
        if (typeof window === 'undefined') {
          return {
            getItem: () => null,
            setItem: () => {},
            removeItem: () => {},
          };
        }
        return localStorage;
      }),
      partialize: (state) => ({
        token: state.token,
        refreshToken: state.refreshToken,
        user: state.user,
        isAuthenticated: state.isAuthenticated,
      }),
      onRehydrateStorage: () => {
        console.log('[AuthStore] Rehydration starting...');
        return (state, error) => {
          if (error) {
            console.error('[AuthStore] Rehydration error:', error);
          } else {
            console.log('[AuthStore] Rehydration complete:', {
              isAuthenticated: state?.isAuthenticated,
              hasToken: !!state?.token,
              hasUser: !!state?.user
            });
            state?.setHasHydrated(true);
          }
        };
      },
    }
  )
);

// Helper to check hydration status
export const useAuthHydration = () => useAuthStore((state) => state._hasHydrated);

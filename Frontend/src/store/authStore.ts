import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { User, UserRole } from '@/types';
import apiClient from '@/lib/api';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  
  // Actions
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  fetchUser: () => Promise<void>;
  
  // Role checks
  isTenantAdmin: () => boolean;
  isBranchAdmin: () => boolean;
  isSalesStaff: () => boolean;
  isTechnicalStaff: () => boolean;
  canManageUsers: () => boolean;
  canManageBranches: () => boolean;
  canViewAllSales: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      isAuthenticated: false,
      isLoading: false,

      login: async (email: string, password: string) => {
        set({ isLoading: true });
        try {
          const response = await apiClient.post('/auth/login', { email, password });
          const { accessToken, refreshToken, user } = response.data.data;
          
          localStorage.setItem('accessToken', accessToken);
          localStorage.setItem('refreshToken', refreshToken);
          
          set({ user, isAuthenticated: true, isLoading: false });
        } catch (error) {
          set({ isLoading: false });
          throw error;
        }
      },

      logout: async () => {
        try {
          await apiClient.post('/auth/logout');
        } finally {
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          set({ user: null, isAuthenticated: false });
        }
      },

      fetchUser: async () => {
        try {
          const response = await apiClient.get('/auth/me');
          set({ user: response.data.data, isAuthenticated: true });
        } catch {
          set({ user: null, isAuthenticated: false });
        }
      },

      isTenantAdmin: () => get().user?.role === UserRole.TenantAdmin,
      isBranchAdmin: () => {
        const role = get().user?.role;
        return role === UserRole.BranchAdmin || role === UserRole.BranchManager;
      },
      isSalesStaff: () => get().user?.role === UserRole.SalesStaff,
      isTechnicalStaff: () => get().user?.role === UserRole.TechnicalStaff,
      canManageUsers: () => {
        const role = get().user?.role;
        return role === UserRole.TenantAdmin || role === UserRole.BranchAdmin;
      },
      canManageBranches: () => get().user?.role === UserRole.TenantAdmin,
      canViewAllSales: () => {
        const role = get().user?.role;
        return role === UserRole.TenantAdmin || role === UserRole.BranchAdmin || role === UserRole.BranchManager;
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({ user: state.user, isAuthenticated: state.isAuthenticated }),
    }
  )
);

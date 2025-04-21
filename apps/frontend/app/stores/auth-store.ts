// stores/authStore.ts
import { create } from 'zustand'

interface AuthState {
  refreshTokenExpiry: number | null;
  setRefreshTokenExpiry: (timestamp: number | null) => void;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  refreshTokenExpiry: null,

  setRefreshTokenExpiry: (timestamp) => {
    set({ refreshTokenExpiry: timestamp });
  },

}));

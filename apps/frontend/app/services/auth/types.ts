export type User = {
  email: string;
  jwt: string;
  userAgent: string | null;
  refreshToken: string;
  refreshTokenExpiry: number;
  id: string;
  jwtExpiry: number | null;
  // ... other user properties
};

// Types for authentication requests and responses
export interface LoginRequest {
  email: string;
  usernameType: string; // username or email
  password: string;
  userAgent: string;
}

export interface AuthResponse {
  token?: string;
  message?: string;
}

export interface AuthError {
  message: string;
}

// Base URL for API calls
export const API_BASE_URL = 'http://localhost:52295';
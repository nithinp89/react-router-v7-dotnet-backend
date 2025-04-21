export type User = {
  email: string;
  jwt: string;
  user_agent: string | null;
  refresh_token: string;
  refresh_token_expiry: number;
  id: string;
  jwt_expiry: number | null;
  // ... other user properties
};

// Types for authentication requests and responses
export interface LoginRequest {
  email: string;
  username_type: string; // username or email
  password: string;
  user_agent: string;
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
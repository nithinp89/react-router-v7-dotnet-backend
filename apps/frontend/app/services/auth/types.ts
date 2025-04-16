export type User = {
  email: string;
  jwt: string;
  id: string;
  // ... other user properties
};

// Types for authentication requests and responses
export interface LoginRequest {
  email: string;
  username_type: string; // username or email
  password: string;
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
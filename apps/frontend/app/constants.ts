export const APP_NAME = 'React Router with Dotnet';

export const APP_DESCRIPTION = 'A sample app using React Router with Dotnet backend';

export const APP_URL = 'http://localhost:3000';

export const APP_LOGO = 'https://reactrouter.com/logo.svg';

export const APP_FAVICON = 'https://reactrouter.com/favicon.ico';

/**
 * Theme
 */
export const APP_THEME_STORAGE_KEY = 'app-theme';

export const APP_THEME_COOKIE_SECRET = "5bd2624bd0464de478a45b8e36319e47fe2b600a4ffe80cad9d5c5e56911ca7b1b63e9c82452163d22ff52aedf0f847bc97c191da8ee5e15ca92d7a1bba1f606";

/**
 * Routes
 */
export const ROUTE_HOME = '/';
export const ROUTE_SET_THEME = '/action/set-theme';

export const ROUTE_AUTH_PREFIX = '/auth';
export const ROUTE_AUTH_LOGIN_PATH = '/login';
export const ROUTE_AUTH_LOGOUT_PATH = '/logout';
export const ROUTE_AUTH_LOGIN = `${ROUTE_AUTH_PREFIX}${ROUTE_AUTH_LOGIN_PATH}`;
export const ROUTE_AUTH_LOGOUT = `${ROUTE_AUTH_PREFIX}${ROUTE_AUTH_LOGOUT_PATH}`;

export const ROUTE_DASHBOARD = '/dashboard';


/**
 * AUTH
 */
export const AUTH_JWT_KEY = "__session";
export const AUTH_FAILED_MESSAGE = "Authentication failed.";
export const AUTH_FAILED_NO_USER_MESSAGE = "No user or JWT retrived after login";
export const AUTH_SERVICE_UNAVAILABLE_MESSAGE = "Authentication Service Unavailable";
export const AUTH_INVALID_CREDENTIALS_MESSAGE = "Invalid Email or Password. Please try again.";
export const AUTH_SUCCESS_MESSAGE = "Authentication successful.";
export const AUTH_NO_USER_JWT_MESSAGE = "No user found in session";
export const AUTH_JWT_EXPIRED_MESSAGE = "JWT has expired (exp claim)";
export const AUTH_JWT_NOT_VALID_YET_MESSAGE = "JWT not valid yet (nbf claim)";
export const AUTH_JWT_ISSUED_IN_FUTURE_MESSAGE = "JWT issued in the future (iat claim)";


/**
 * BACKEND API
 */
export const API_BASE_URL = 'http://localhost:52295';
export const API_AUTH_LOGIN = '/auth/login';
export const API_AUTH_LOGOUT = '/auth/logout';





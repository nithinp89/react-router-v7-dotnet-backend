export const APP_NAME = 'React Router with Dotnet';

export const APP_DESCRIPTION = 'A sample app using React Router with Dotnet backend';

export const APP_URL = 'http://localhost:3000';

export const APP_LOGO = 'https://reactrouter.com/logo.svg';

export const APP_FAVICON = 'https://reactrouter.com/favicon.ico';

/**
 * Theme
 */
export enum Theme {
  STORAGE_KEY = 'app_theme',
  COOKIE_SECRET = "5bd2624bd0464de478a45b8e36319e47fe2b600a4ffe80cad9d5c5e56911ca7b1b63e9c82452163d22ff52aedf0f847bc97c191da8ee5e15ca92d7a1bba1f606",
}

/**
 * Routes
 */
export enum Routes {
  HOME = '/',
  SET_THEME = '/action/set-theme',
  AUTH_PREFIX = '/auth',
  AUTH_LOGIN_PATH = '/login',
  AUTH_LOGOUT_PATH = '/logout',
  AUTH_LOGIN = `${Routes.AUTH_PREFIX}${Routes.AUTH_LOGIN_PATH}`,
  AUTH_LOGOUT = `${Routes.AUTH_PREFIX}${Routes.AUTH_LOGOUT_PATH}`,
  AUTH_SESSION_RENEWER_PATH = '/session/renew',
  AUTH_SESSION_RENEWER = `${Routes.AUTH_PREFIX}${Routes.AUTH_SESSION_RENEWER_PATH}`,
  DASHBOARD = '/dashboard',
}

/**
 * AUTH
 */
export enum Auth {
  SESSION_KEY = "__session",
  FAILED = "Authentication failed.",
  FAILED_NO_USER = "No user or JWT retrived after login.",
  SERVICE_UNAVAILABLE = "Authentication Service Unavailable.",
  INVALID_CREDENTIALS = "Invalid Email or Password. Please try again.",
  SUCCESS = "Authentication successful.",
  NO_USER_JWT = "No user found in session.",
  SESSION_INACTIVE = "Session is not active.",
  SESSION_USER_AGENT_MISMATCHES = "User Agents are not matching.",
  SESSION_RENEWAL_ERROR = "Session renewal error.",
  SESSION_RENEWAL_FAILED = "Session renewal failed.",
  SESSION_RENEWAL_EXPIRED = "Session renewal expired.",
  SESSION_RENEWAL_SUCCESS = "Session renewal successful.",
  JWT_EXPIRED = "JWT has expired (exp claim).",
  JWT_NOT_VALID_YET = "JWT not valid yet (nbf claim).",
  JWT_ISSUED_IN_FUTURE = "JWT issued in the future (iat claim).",
  EMPTY_USER_UUID = "00000000-0000-0000-0000-000000000000",
}

export enum Toast {
  ERROR = "error",
  INFO = "info",
  SUCCESS = "success",
  WARNING = "warning",
}


/**
 * BACKEND API
 */
export enum BackendApi {
  BASE_URL = 'http://localhost:52295',
  AUTH_LOGIN = '/auth/login',
  AUTH_LOGOUT = '/auth/logout',
  AUTH_RENEW_SESSION = '/auth/renew-session',
}





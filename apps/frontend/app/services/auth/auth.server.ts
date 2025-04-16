/**
 * Authentication service for handling API calls to the authentication endpoints
 */
import jwt from "jsonwebtoken";
import type { User } from "./types";
import type { LoginRequest } from "./types";
import type { AuthResponse } from "./types";
import { API_BASE_URL, API_AUTH_LOGIN, API_AUTH_LOGOUT, ROUTE_AUTH_LOGIN } from "~/constants";
import { createCookieSessionStorage } from "react-router";
import logger from "~/services/logger/logger.server";
import { redirect } from "react-router";
import { AUTH_JWT_KEY, AUTH_FAILED_MESSAGE, AUTH_INVALID_CREDENTIALS_MESSAGE, AUTH_FAILED_NO_USER_MESSAGE, AUTH_SUCCESS_MESSAGE, AUTH_SERVICE_UNAVAILABLE_MESSAGE, AUTH_NO_USER_JWT_MESSAGE, AUTH_JWT_NOT_VALID_YET_MESSAGE, AUTH_JWT_EXPIRED_MESSAGE, AUTH_JWT_ISSUED_IN_FUTURE_MESSAGE } from "~/constants";

// Create a session storage
export const sessionStorage = createCookieSessionStorage({
  cookie: {
    name: AUTH_JWT_KEY,
    httpOnly: true,
    path: "/",
    sameSite: "lax",
    secrets: ["s3cr3t90"], // replace this with an actual secret -- TODO
    secure: process.env.NODE_ENV === "production",
  },
});

export const AuthService = {

  /**
 * Login a user with username and password
 * @param credentials - The login credentials
 * @returns A promise with the fetch response
 */
  async loginRequest(credentials: LoginRequest): Promise<User> {
    try {
      const response = await fetch(`${API_BASE_URL}${API_AUTH_LOGIN}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(credentials),
        credentials: 'include', // This ensures cookies are sent and stored
      });

      logger.debug("Authentication response status:", { status: response.status });
      logger.debug(response.ok);

      if (!response.ok) {
        // Handle authentication failure
        const errorText = await response.text();
        logger.info(AUTH_FAILED_MESSAGE, { errorText });

        if (response.status === 401) {
          logger.warn(AUTH_INVALID_CREDENTIALS_MESSAGE, credentials.email);
          throw new Error(AUTH_INVALID_CREDENTIALS_MESSAGE);
        }

        throw new Error(AUTH_FAILED_MESSAGE);
      }

      const data = await response.json();
      logger.debug(AUTH_SUCCESS_MESSAGE, { data });

      if (!data.email || !data.jwt || !data.id) {
        logger.error(AUTH_FAILED_NO_USER_MESSAGE, { data });
        throw new Error(AUTH_FAILED_NO_USER_MESSAGE);
      }

      const user: User = { email: data.email, jwt: data.jwt, id: data.id };
      return user;

    } catch (error: any) {
      // Only catch and wrap errors that are NOT the explicit 401 error
      if (error instanceof Error && error.message === AUTH_INVALID_CREDENTIALS_MESSAGE) {
        throw error; // Let 401 errors bubble up
      }

      logger.error(AUTH_SERVICE_UNAVAILABLE_MESSAGE, { error });
      throw new Error(AUTH_SERVICE_UNAVAILABLE_MESSAGE);
    }
  },

  /**
   * Logout the current user
   * @returns A promise with the logout response
   */
  async logout(request: Request): Promise<Response> {
    try {
      await fetch(`${API_BASE_URL}${API_AUTH_LOGOUT}`, {
        method: 'POST',
        credentials: 'include',
      });

    } catch (error) {
      logger.error(AUTH_SERVICE_UNAVAILABLE_MESSAGE, { error });
    }

    let session = await sessionStorage.getSession(request.headers.get("cookie"));
    return redirect(ROUTE_AUTH_LOGIN, {
      headers: { "Set-Cookie": await sessionStorage.destroySession(session) },
    });
  },

  /**
   * Get the current user's information
   * @returns A promise with the user information
   */
  async getCurrentUserOrRedirect(request: Request): Promise<User | Response> {
    try {
      const user = await this.getCurrentUser(request);

      if (!user) {
        throw new Error(AUTH_NO_USER_JWT_MESSAGE);
      }

      return user;
    } catch (error) {
      logger.info(AUTH_NO_USER_JWT_MESSAGE, { error });

      let session = await sessionStorage.getSession(request.headers.get("cookie"));
      return redirect(ROUTE_AUTH_LOGIN, {
        headers: { "Set-Cookie": await sessionStorage.destroySession(session) },
      });
    }
  },


  /**
   * Get the current user's information
   * @returns A promise with the user information
   */
  async getCurrentUser(request: Request): Promise<User | null> {
    try {
      const session = await sessionStorage.getSession(request.headers.get("cookie"));
      const user = session.get("user");

      logger.debug("Current user:", { user });

      if (!user || !user.jwt) {
        logger.debug(AUTH_NO_USER_JWT_MESSAGE, { user });
        return null;
      }

      const decoded: any = jwt.decode(user.jwt);
      logger.debug("Decoded JWT payload:", decoded);

      const now = Math.floor(Date.now() / 1000); // current time in seconds

      if (decoded.nbf && now < decoded.nbf) {
        throw new Error(AUTH_JWT_NOT_VALID_YET_MESSAGE);
      }
      if (decoded.exp && now >= decoded.exp) {
        throw new Error(AUTH_JWT_EXPIRED_MESSAGE);
      }
      if (decoded.iat && now < decoded.iat) {
        throw new Error(AUTH_JWT_ISSUED_IN_FUTURE_MESSAGE);
      }

      return { ...user, decodedJwt: decoded };

    } catch (error) {
      logger.warn(AUTH_NO_USER_JWT_MESSAGE, { error });

      return null;
    }
  },

  /**
 * Check if the user is authenticated
 * @returns True if the user is authenticated, false otherwise
 */
  async isAuthenticated(request: Request): Promise<boolean> {
    const user = await this.getCurrentUser(request);
    return !!user;
  },
};
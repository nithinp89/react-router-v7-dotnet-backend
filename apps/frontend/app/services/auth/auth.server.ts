/**
 * Authentication service for handling API calls to the authentication endpoints
 */
import jwt from "jsonwebtoken";
import type { User } from "./types";
import type { LoginRequest } from "./types";
import type { AuthResponse } from "./types";
import { BackendApi } from "~/constants";
import { createCookieSessionStorage } from "react-router";
import logger from "~/services/logger/logger.server";
import { redirect } from "react-router";
import { Auth, Routes, Headers } from "~/constants";
import { v4 as uuidv4 } from 'uuid';

// Create a session storage
export const sessionStorage = createCookieSessionStorage({
  cookie: {
    name: Auth.SESSION_KEY,
    httpOnly: true,
    path: "/",
    sameSite: "lax",
    secrets: ["s3cr3t90"], // replace this with an actual secret -- TODO
    secure: process.env.NODE_ENV === "production",
    // DO NOT SET maxAge to keep active only until browser closes
  },
});

export const AuthService = {

  /**
 * Login a user with username and password
 * @param credentials - The login credentials
 * @returns A promise with the fetch response
 */
  async loginRequest(request: Request, credentials: LoginRequest): Promise<User> {
    try {
      const response = await fetch(`${BackendApi.BASE_URL}${BackendApi.AUTH_LOGIN}`, {
        method: 'POST',
        headers: {
          [Headers.CONTENT_TYPE]: Headers.CONTENT_TYPE_JSON,
          [Headers.X_REQUEST_ID]: request.headers.get(Headers.X_REQUEST_ID) as string ?? uuidv4(),
        },
        body: JSON.stringify(credentials)
      });

      logger.debug("Authentication response status:", { status: response.status });
      logger.debug(response.ok);

      if (!response.ok) {
        // Handle authentication failure
        const errorText = await response.text();
        logger.info(Auth.FAILED, { errorText });

        if (response.status === 401) {
          logger.warn(Auth.INVALID_CREDENTIALS, credentials.email);
          throw new Error(Auth.INVALID_CREDENTIALS);
        }

        throw new Error(Auth.FAILED);
      }

      const data = await response.json();
      logger.debug(Auth.SUCCESS, { data });

      if (!data.email || !data.jwt || !data.id) {
        logger.error(Auth.FAILED_NO_USER, { data });
        throw new Error(Auth.FAILED_NO_USER);
      }

      const decoded: any = jwt.decode(data.jwt);
      logger.debug("Decoded JWT payload:", decoded);

      const user: User = { 
        email: data.email, 
        jwt: data.jwt, 
        jwtExpiry: decoded.exp,
        refreshToken: data.refreshToken,
        refreshTokenExpiry: data.refreshTokenExpiry,
        userAgent: credentials.userAgent,
        id: data.id };

      return user;

    } catch (error: any) {
      // Only catch and wrap errors that are NOT the explicit 401 error
      if (error instanceof Error && error.message === Auth.INVALID_CREDENTIALS) {
        throw error; // Let 401 errors bubble up
      }

      logger.error(Auth.SERVICE_UNAVAILABLE, { error });
      throw new Error(Auth.SERVICE_UNAVAILABLE);
    }
  },

  /**
   * Logout the current user
   * @returns A promise with the logout response
   */
  async logout(request: Request): Promise<Response> {
    try {
      await fetch(`${BackendApi.BASE_URL}${BackendApi.AUTH_LOGOUT}`, {
        method: 'POST',
        credentials: 'include',
      });

    } catch (error) {
      logger.error(Auth.SERVICE_UNAVAILABLE, { error });
    }

    let session = await sessionStorage.getSession(request.headers.get("cookie"));
    return redirect(BackendApi.AUTH_LOGIN, {
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
        throw new Error(Auth.NO_USER_JWT);
      }

      return user;
    } catch (error) {
      logger.info(Auth.NO_USER_JWT, { error });

      let session = await sessionStorage.getSession(request.headers.get("cookie"));
      return redirect(Routes.AUTH_LOGIN, {
        headers: { "Set-Cookie": await sessionStorage.destroySession(session) }
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
        logger.debug(Auth.NO_USER_JWT, { user });
        return null;
      }

      const decoded: any = jwt.decode(user.jwt);
      logger.debug("Decoded JWT payload:", decoded);

      const now = Math.floor(Date.now() / 1000); // current time in seconds

      if (decoded.nbf && now < decoded.nbf) {
        const err = new Error(Auth.JWT_NOT_VALID_YET) as Error & { user?: User };
        err.user = user;
        throw err;
      }
      if (decoded.exp && now >= decoded.exp) {
        const err = new Error(Auth.JWT_EXPIRED) as Error & { user?: User };
        err.user = user;
        throw err;
      }
      if (decoded.iat && now < decoded.iat) {
        const err = new Error(Auth.JWT_ISSUED_IN_FUTURE) as Error & { user?: User };
        err.user = user;
        throw err;
      }

      return { ...user, decodedJwt: decoded };

    } catch (error: any) {
      logger.warn(Auth.SESSION_INACTIVE, error);
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
  
  /**
   * Renew Jwt with Refresh Token
   * @param user
   */
  async renewSession(request: Request, user: User): Promise<User | null> {
    try {
      if (!user) {
        throw new Error(Auth.NO_USER_JWT);
      }

      // Renew if the browser is same!
      if(request.headers.get("user-agent") != user.userAgent) {
        throw new Error(Auth.SESSION_USER_AGENT_MISMATCHES);
      }
      console.log(JSON.stringify(user));
      const response = await fetch(`${BackendApi.BASE_URL}${BackendApi.AUTH_RENEW_SESSION}`, {
        method: 'POST',
        headers: {
          [Headers.CONTENT_TYPE]: Headers.CONTENT_TYPE_JSON,
          [Headers.X_REQUEST_ID]: request.headers.get(Headers.X_REQUEST_ID) as string,
        },
        body: JSON.stringify({
          Email: user.email,
          Jwt: user.jwt,
          RefreshToken: user.refreshToken,
          RefreshTokenExpiry: user.refreshTokenExpiry,
          Id: user.id,
        })
      });

      logger.debug("Session Renew response status:", { status: response.status });
      logger.debug(response.ok);

      if (!response.ok) {
        // Handle authentication failure
        const errorText = await response.text();
        logger.info(Auth.SESSION_RENEWAL_FAILED, { errorText });

        if (response.status === 401) {
          logger.warn(Auth.SESSION_RENEWAL_EXPIRED, user.email);
          throw new Error(Auth.SESSION_RENEWAL_EXPIRED);
        }

        throw new Error(Auth.SESSION_RENEWAL_FAILED);
      }

      const data = await response.json();
      logger.debug(Auth.SESSION_RENEWAL_SUCCESS, { data });

      if (!data.email || !data.jwt || !data.id) {
        logger.error(Auth.FAILED_NO_USER, { data });
        throw new Error(Auth.FAILED_NO_USER);
      }

      const decoded: any = jwt.decode(data.jwt);
      logger.debug("Decoded JWT payload:", decoded);

      const userData: User = { 
        email: data.email, 
        jwt: data.jwt, 
        jwtExpiry: decoded.exp,
        refreshToken: data.refresh_token,
        refreshTokenExpiry: data.refresh_token_expiry,
        userAgent: request.headers.get("user-agent"),
        id: data.id };

      return userData;

      
    } catch (error) {
      logger.error(Auth.SESSION_RENEWAL_ERROR, { error });
      return null;
    }
  }
};
/**
 * Authentication service for handling API calls to the authentication endpoints
 */
import jwt from "jsonwebtoken";
import type { User } from "./types";
import type { LoginRequest } from "./types";
import type { AuthResponse } from "./types";
import type { AuthError } from "./types";
import { API_BASE_URL } from "./types";
import { createCookieSessionStorage } from "react-router";
import logger from "~/services/logger/logger.server";
import { redirect } from "react-router";

// Create a session storage
export const sessionStorage = createCookieSessionStorage({
  cookie: {
    name: "__session",
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
      const response = await fetch(`${API_BASE_URL}/auth/login`, {
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
        logger.info("Authentication failed:", { errorText });

        if (response.status === 401) {
          logger.debug("Invalid Email or Password. Please try again.");
          throw new Error("Invalid Email or Password. Please try again.");
        }

        throw new Error("Authentication failed");
      }

      const data = await response.json();
      logger.debug("Authentication successful", { data });

      if (!data.email || !data.jwt || !data.id) {
        logger.error("No user or JWT retrived after login", { data });
        throw new Error('No user or JWT retrived after login');
      }

      const user: User = { email: data.email, jwt: data.jwt, id: data.id };
      return user;

    } catch (error: any) {
      // Only catch and wrap errors that are NOT the explicit 401 error
      if (error instanceof Error && error.message === "Invalid Email or Password. Please try again.") {
        throw error; // Let 401 errors bubble up
      }

      logger.error("Failed to make login request", { error });
      throw new Error("Authentication Service Unavailable");
    }
  },

  /**
   * Logout the current user
   * @returns A promise with the logout response
   */
  async logout(request: Request): Promise<Response> {
    try {
      await fetch(`${API_BASE_URL}/auth/logout`, {
        method: 'POST',
        credentials: 'include',
      });

    } catch (error) {
      logger.error('Logout error:', { error });
    }

    let session = await sessionStorage.getSession(request.headers.get("cookie"));
    return redirect("/", {
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
        throw new Error('No user found in session');
      }

      return user;
    } catch (error) {
      logger.error('Get current user or redirect error:', { error });

      let session = await sessionStorage.getSession(request.headers.get("cookie"));
      return redirect("/auth/login", {
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
        throw new Error('No user or JWT found in session');
      }

      const decoded: any = jwt.decode(user.jwt);
      logger.debug("Decoded JWT payload:", decoded);

      const now = Math.floor(Date.now() / 1000); // current time in seconds

      if (decoded.nbf && now < decoded.nbf) {
        throw new Error('JWT not valid yet (nbf claim)');
      }
      if (decoded.exp && now >= decoded.exp) {
        throw new Error('JWT expired (exp claim)');
      }
      if (decoded.iat && now < decoded.iat) {
        throw new Error('JWT issued in the future (iat claim)');
      }

      return { ...user, decodedJwt: decoded };

    } catch (error) {
      logger.error('Get current user error:', { error });

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
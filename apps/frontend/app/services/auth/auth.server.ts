/**
 * Authentication service for handling API calls to the authentication endpoints
 */
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
    secrets: ["s3cr3t"], // replace this with an actual secret -- TODO
    secure: process.env.NODE_ENV === "production",
  },
});

export const AuthService = {

  /**
 * Login a user with username and password
 * @param credentials - The login credentials
 * @returns A promise with the fetch response
 */
  async loginRequest(credentials: LoginRequest): Promise<Response> {
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

      return response;

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
  async getCurrentUser(request: Request): Promise<User | null> {
    try {
      const session = await sessionStorage.getSession(request.headers.get("cookie"));
      const user = session.get("__session"); // or whatever key you store user info under
      if (!user) {
        throw new Error('No user found in session');
      }
      return user;
    } catch (error) {
      logger.error('Get current user error:', { error });
      return null;
    }

    /* const response = await fetch(`${API_BASE_URL}/auth/me`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
      },
      credentials: 'include',
    }); */


  },

  /**
 * Check if the user is authenticated
 * @returns True if the user is authenticated, false otherwise
 */
  async isAuthenticated(request: Request): Promise<boolean> {
    const session = await sessionStorage.getSession(request.headers.get("cookie"));
    return session.has("__session");
  },
};
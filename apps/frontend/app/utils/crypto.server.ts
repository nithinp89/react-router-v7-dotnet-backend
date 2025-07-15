import * as crypto from 'crypto';

const COOKIE_SECRET = "s3cr3t-k3y-f0r-c00k13-3ncrypt10n"; // Should be in environment variables

/**
 * Encrypt data using AES-256-GCM
 * @param data Data to encrypt
 * @returns Encrypted data as base64 string
 */
function encrypt(data: string): string {
  const iv = crypto.randomBytes(16);
  const key = crypto.createHash('sha256').update(COOKIE_SECRET).digest();
  const cipher = crypto.createCipheriv('aes-256-gcm', key, iv);
  
  let encrypted = cipher.update(data, 'utf8', 'base64');
  encrypted += cipher.final('base64');
  
  const authTag = cipher.getAuthTag().toString('base64');
  console.info("Encrypted data:", `${iv.toString('base64')}:${authTag}:${encrypted}`);
  // Format: base64(iv):base64(authTag):base64(encryptedData)
  return `${iv.toString('base64')}:${authTag}:${encrypted}`;
}

/**
 * Decrypt data encrypted with AES-256-GCM
 * @param encryptedData Encrypted data as base64 string
 * @returns Decrypted data
 */
function decrypt(encryptedData: string): string | null {
  try {
    const [ivString, authTagString, encryptedString] = encryptedData.split(':');
    
    if (!ivString || !authTagString || !encryptedString) {
      return null;
    }
    
    const iv = Buffer.from(ivString, 'base64');
    const authTag = Buffer.from(authTagString, 'base64');
    const key = crypto.createHash('sha256').update(COOKIE_SECRET).digest();
    
    const decipher = crypto.createDecipheriv('aes-256-gcm', key, iv);
    decipher.setAuthTag(authTag);
    
    let decrypted = decipher.update(encryptedString, 'base64', 'utf8');
    decrypted += decipher.final('utf8');
    
    return decrypted;
    console.info("Decrypted data:", decrypted);
  } catch (error) {
    console.error('Decryption error:', error);
    return null;
  }
}

/**
 * Create a cookie string with the given value
 * @param value Value to store in the cookie
 * @param options Cookie options
 * @returns Cookie string
 */
function createCookieString(value: string, options: { maxAge?: number, httpOnly?: boolean, secure?: boolean, path?: string, sameSite?: 'strict' | 'lax' | 'none' } = {}): string {
  const encryptedValue = encrypt(value);
  
  const cookieParts = [`${AUTH_COOKIE_NAME}=${encodeURIComponent(encryptedValue)}`];
  
  if (options.maxAge !== undefined) {
    cookieParts.push(`Max-Age=${options.maxAge}`);
  }
  
  if (options.httpOnly) {
    cookieParts.push('HttpOnly');
  }
  
  if (options.secure) {
    cookieParts.push('Secure');
  }
  
  if (options.path) {
    cookieParts.push(`Path=${options.path}`);
  }
  
  if (options.sameSite) {
    cookieParts.push(`SameSite=${options.sameSite}`);
  }
  
  return cookieParts.join('; ');
}

/**
 * Parse a cookie value from a cookie header
 * @param cookieHeader Cookie header string
 * @returns Parsed cookie value or null
 */
function parseCookie(cookieHeader: string): string | null {
  const cookies = cookieHeader.split(';').map(cookie => cookie.trim());
  
  for (const cookie of cookies) {
    if (cookie.startsWith(`${AUTH_COOKIE_NAME}=`)) {
      const encryptedValue = decodeURIComponent(cookie.substring(AUTH_COOKIE_NAME.length + 1));
      return decrypt(encryptedValue);
    }
  }
  
  return null;
}

/**
 * Get the authenticated user from the request
 * @param request The request object
 * @returns The JWT token if authenticated, null otherwise
 */
export async function getAuthenticatedUser(request: Request): Promise<string | null> {
  const cookieHeader = request.headers.get("Cookie");
  if (!cookieHeader) return null;
  
  return parseCookie(cookieHeader);
}

/**
 * Check if the user is authenticated
 * @param request The request object
 * @returns True if authenticated, false otherwise
 */
export async function isAuthenticated(request: Request): Promise<boolean> {
  const token = await getAuthenticatedUser(request);
  return !!token;
}

/**
 * Require authentication for a route
 * @param request The request object
 * @param redirectTo The path to redirect to if not authenticated
 */
export async function requireAuth(
  request: Request,
  redirectTo: string = "/auth/login"
): Promise<string> {
  const token = await getAuthenticatedUser(request);
  
  if (!token) {
    throw redirect(redirectTo);
  }
  
  return token;
}

/**
 * Create a response with the auth cookie set
 * @param token The JWT token
 * @param redirectTo The path to redirect to
 */
export async function createAuthSession(token: string, redirectTo: string = "/"): Promise<Response> {
  const cookieString = createCookieString(token, {
    maxAge: COOKIE_MAX_AGE,
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    path: "/",
    sameSite: "lax"
  });
  
  return new Response(null, {
    status: 302,
    headers: {
      Location: redirectTo,
      "Set-Cookie": cookieString,
    },
  });
}

/**
 * Logout the user by clearing the auth cookie
 * @param redirectTo The path to redirect to after logout
 */
export async function logout(redirectTo: string = "/auth/login"): Promise<Response> {
  const cookieString = createCookieString("", {
    maxAge: 0,
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    path: "/",
    sameSite: "lax"
  });
  
  return new Response(null, {
    status: 302,
    headers: {
      Location: redirectTo,
      "Set-Cookie": cookieString,
    },
  });
}

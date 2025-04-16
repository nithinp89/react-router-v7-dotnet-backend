import { createThemeSessionResolver } from "remix-themes";
import { createCookieSessionStorage } from "react-router";
import { APP_THEME_STORAGE_KEY, APP_THEME_COOKIE_SECRET } from "~/constants";

// You can default to 'development' if process.env.NODE_ENV is not set
const isProduction = process.env.NODE_ENV === "production"

const sessionStorage = createCookieSessionStorage({
  cookie: {
    name: APP_THEME_STORAGE_KEY,
    path: "/",
    httpOnly: true,
    sameSite: "lax",
    secrets: [APP_THEME_COOKIE_SECRET],
    // Set domain and secure only if in production
    ...(isProduction
      ? { domain: process.env.APP_DOMAIN, secure: true }
      : {}),
  },
})

export const themeSessionResolver = createThemeSessionResolver(sessionStorage)

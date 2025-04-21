import { createThemeSessionResolver } from "remix-themes";
import { createCookieSessionStorage } from "react-router";
import { Theme } from "~/constants";

const sessionStorage = createCookieSessionStorage({
  cookie: {
    name: Theme.STORAGE_KEY,
    path: "/",
    httpOnly: true,
    sameSite: "lax",
    secrets: [Theme.COOKIE_SECRET],
    // Set domain and secure only if in production
    secure: process.env.NODE_ENV === "production",
    // Make cookie persist for 1 year
    maxAge: 60*60*24*365,
  },
})

export const themeSessionResolver = createThemeSessionResolver(sessionStorage)

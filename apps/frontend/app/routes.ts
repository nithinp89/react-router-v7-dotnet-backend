import { type RouteConfig, index, route, layout, prefix } from "@react-router/dev/routes";
import { Routes } from "./constants";

export default [
  index("routes/home.tsx"),
  route("/dashboard", "routes/dashboard.tsx"),

  // Auth
  ...prefix(Routes.AUTH_PREFIX, [
    layout("routes/auth/auth-layout.tsx", [
      route(Routes.AUTH_LOGIN_PATH, "routes/auth/login.tsx"),
      route(Routes.AUTH_LOGOUT_PATH, "routes/auth/logout.tsx"),
      route(Routes.AUTH_SESSION_RENEWER_PATH, "routes/auth/session-renewer.tsx"),
    ]),
  ]),

  // Theme
  route(Routes.SET_THEME, "routes/theme/set-theme.ts"),
] satisfies RouteConfig;

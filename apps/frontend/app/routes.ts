import { type RouteConfig, index, route, layout, prefix } from "@react-router/dev/routes";
import { ROUTE_SET_THEME, ROUTE_AUTH_PREFIX, ROUTE_AUTH_LOGIN, ROUTE_AUTH_LOGOUT } from "~/constants";

export default [
  index("routes/home.tsx"),
  route("/dashboard", "routes/dashboard.tsx"),

  // Auth
  ...prefix(ROUTE_AUTH_PREFIX, [
    layout("routes/auth/auth-layout.tsx", [
      route(ROUTE_AUTH_LOGIN, "routes/auth/login.tsx"),
      route(ROUTE_AUTH_LOGOUT, "routes/auth/logout.tsx"),
    ]),
  ]),

  // Theme
  route(ROUTE_SET_THEME, "routes/theme/set-theme.ts"),
] satisfies RouteConfig;

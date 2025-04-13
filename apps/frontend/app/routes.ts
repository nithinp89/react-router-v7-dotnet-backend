import { type RouteConfig, index, route, layout, prefix } from "@react-router/dev/routes";

export default [
    index("routes/home.tsx"),
    route("/dashboard", "routes/dashboard.tsx"),

    // Auth
    ...prefix("auth", [
    layout("routes/auth/auth-layout.tsx", [
        route("/login", "routes/auth/login.tsx"),
    ]),]),

] satisfies RouteConfig;

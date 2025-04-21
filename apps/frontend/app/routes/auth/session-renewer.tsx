import { Form, useActionData, redirect, useNavigation } from "react-router";
import { authenticator } from "~/services/auth/authenticator.server";
import { sessionStorage, AuthService } from "~/services/auth/auth.server";
import type { Route } from "./+types/login";
import logger from "~/services/logger/logger.server";
import { LoginForm } from "~/components/auth/login-form";
import { Routes, Toast } from "~/constants";
import { toast } from "sonner";
import { useEffect } from "react";
import type { User } from "~/services/auth/types";

export function meta({ }: Route.MetaArgs) {
  return [
    { title: "Session Renewer" },
    { name: "description", content: "Renew the session silently with new jwt" },
  ];
}

export async function loader({ request }: Route.LoaderArgs) {
  return null;
}

export async function action({ request }: Route.ActionArgs) {
  const userOrResponse = await AuthService.getCurrentUserOrRedirect(request);
  if (userOrResponse instanceof Response) {
    return userOrResponse;
  }

  try {
    let user = await AuthService.renewSession(request, userOrResponse as User);
    if (!user) {
      return null;
    }

    let session = await sessionStorage.getSession(
      request.headers.get("cookie")
    );
    logger.info("Session Renewed for user", { user });

    session.set("user", user);

    // Redirect to the home page after successful login
    const setCookieHeader = await sessionStorage.commitSession(session);
    const headers = new Headers();
    headers.append("Set-Cookie", setCookieHeader);
    headers.append("Content-Type", "application/json");

    // Return the user as JSON for client-side state update
    return new Response(JSON.stringify({ refresh_token_expiry: user?.refresh_token_expiry }), {
      status: 200,
      headers: headers,
    });
  } catch (error) {
    return null;
  }
}

// export default function SessionRenewerPage() {
//   return null;
// }
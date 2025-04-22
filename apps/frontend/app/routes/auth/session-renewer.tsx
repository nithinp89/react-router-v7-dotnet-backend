import { sessionStorage, AuthService } from "~/services/auth/auth.server";
import type { Route } from "./+types/login";
import logger from "~/services/logger/logger.server";
import { Auth } from "~/constants";
import type { User } from "~/services/auth/types";

export async function action({ request }: Route.ActionArgs) {
  const userOrResponse = await AuthService.getCurrentUserOrRedirect(request);
  if (userOrResponse instanceof Response) {
    return userOrResponse;
  }

  try {
    const user = await AuthService.renewSession(request, userOrResponse as User);
    if (!user) {
      throw new Error(Auth.SESSION_RENEWAL_NO_USER_FOUND);
    }

    const session = await sessionStorage.getSession(
      request.headers.get("cookie")
    );
    logger.info(Auth.SESSION_RENEWAL_SUCCESS, { userId: user?.id });

    session.set("user", user);

    // Redirect to the home page after successful login
    const setCookieHeader = await sessionStorage.commitSession(session);
    const headers = new Headers();
    headers.append("Set-Cookie", setCookieHeader);
    headers.append("Content-Type", "application/json");

    // Return the user as JSON for client-side state update
    return new Response(JSON.stringify({ refreshTokenExpiry: user?.refreshTokenExpiry }), {
      status: 200,
      headers: headers,
    });

  } catch (error) {
    logger.error(Auth.SESSION_RENEWAL_FAILED, { error, userId: userOrResponse?.id });
    return new Response(null, { status: 500 });
  }
}
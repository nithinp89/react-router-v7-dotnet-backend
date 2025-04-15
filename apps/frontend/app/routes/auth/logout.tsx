import type { Route } from "./+types/logout";
import { AuthService } from "~/services/auth/auth.server";
import logger from "~/services/logger/logger.server";


/**
 * Server-side action function for handling logout
 */
export async function action({ request }: Route.ActionArgs): Promise<Response> {
  logger.debug("Logout request received");
  return await AuthService.logout(request);
}

/**
 * Server-side loader function for handling logout
 */
export async function loader({ request }: Route.ActionArgs): Promise<Response> {
  logger.debug("Logout request received");
  return await AuthService.logout(request);
}

/**
 * Logout page component - this should never be rendered
 * as the loader will always redirect
 */
export default function LogoutPage() {
  return (
    <div>Logging out...</div>
  );
}

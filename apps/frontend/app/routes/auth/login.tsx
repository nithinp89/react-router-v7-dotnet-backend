import { Form, useActionData, redirect, useNavigation } from "react-router";
import { authenticator } from "~/services/auth/authenticator.server";
import { sessionStorage, AuthService } from "~/services/auth/auth.server";
import type { Route } from "./+types/login";
import logger from "~/services/logger/logger.server";
import { LoginForm } from "~/components/auth/login-form";
import { Routes, Toast } from "~/constants";
import { toast } from "sonner";
import { useEffect } from "react";

export function meta({ }: Route.MetaArgs) {
  return [
    { title: "Login" },
    { name: "description", content: "Login to App!" },
  ];
}

export async function loader({ request }: Route.LoaderArgs) {
  // Check if user is already authenticated
  const isAuthenticated = await AuthService.isAuthenticated(request);

  // If authenticated, redirect to dashboard
  if (isAuthenticated) {
    logger.debug("User already authenticated, redirecting to dashboard");
    return redirect(Routes.DASHBOARD);
  }

  // Otherwise, continue to login page
  return null;
}

export async function action({ request }: Route.ActionArgs) {
  try {
    // we call the method with the name of the strategy we want to use and the
    // request object
    let user = await authenticator.authenticate("user-pass", request);

    let session = await sessionStorage.getSession(
      request.headers.get("cookie")
    );
    logger.info("User logged in", { user });

    session.set("user", user);

    // Redirect to the home page after successful login
    const setCookieHeader = await sessionStorage.commitSession(session);    
    const headers = new Headers();
    headers.append("Set-Cookie", setCookieHeader);

    return redirect(Routes.DASHBOARD, {
      headers,
    });
  } catch (error) {
    // Return validation errors or authentication errors
    if (error instanceof Error) {
      return ({ error: error.message });
    }

    // Re-throw any other errors (including redirects)
    throw error;
  }
}

export default function LoginPage() {
  const actionData = useActionData<typeof action>();
  const navigation = useNavigation();
  const isLoading = navigation.state === "submitting";

  useEffect(() => {
    if (actionData?.error) {
      toast.error(actionData.error);
    }
  }, [actionData]);

  return (

    <div className="flex min-h-svh flex-col items-center justify-center bg-muted p-6 md:p-10">
      <div className="w-full max-w-sm md:max-w-3xl">
        <LoginForm isLoading={isLoading} />
      </div>
    </div>
  );
}
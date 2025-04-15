import { Form, useActionData, redirect, useNavigation } from "react-router";
import { authenticator } from "~/services/auth/authenticator.server";
import { sessionStorage } from "~/services/auth/auth.server";
import type { Route } from "./+types/login";
import logger from "~/services/logger/logger.server";
import { LoginForm } from "~/components/auth/login-form";

export function meta({ }: Route.MetaArgs) {
  return [
    { title: "Login" },
    { name: "description", content: "Login to App!" },
  ];
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
    return redirect("/dashboard", {
      headers: {
        "Set-Cookie": await sessionStorage.commitSession(session),
      },
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
  return (

    <div className="flex min-h-svh flex-col items-center justify-center bg-muted p-6 md:p-10">
          <div className="w-full max-w-sm md:max-w-3xl">
            <LoginForm error={actionData?.error ?? null} isLoading={isLoading} />
          </div>
        </div>

    // <div>
    //   <h1>Login</h1>

    //   {actionData?.error ? (
    //     <div className="error">{actionData.error}</div>
    //   ) : null}

    //   <Form method="post">
    //     <div>
    //       <label htmlFor="email">Email</label>
    //       <input type="text" name="email" id="email" required />
    //     </div>

    //     <div>
    //       <label htmlFor="password">Password</label>
    //       <input
    //         type="password"
    //         name="password"
    //         id="password"
    //         autoComplete="current-password"
    //         required
    //       />
    //     </div>

    //     <button type="submit">Sign In</button>
    //   </Form>
    // </div>
  );
}
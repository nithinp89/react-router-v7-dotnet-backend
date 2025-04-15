// app/services/auth.server.ts
import { Authenticator } from "remix-auth";
import { FormStrategy } from "remix-auth-form";
import type { User } from "./types";
import { AuthService } from "./auth.server";
import logger from "~/services/logger/logger.server";

// Create an instance of the authenticator, pass a generic with what
// strategies will return
export const authenticator = new Authenticator<User>();

// Tell the Authenticator to use the form strategy
authenticator.use(
    new FormStrategy(async ({ form }) => {
        const email = form.get("email") as string;
        const password = form.get("password") as string;

        if (!email || !password) {
            throw new Error("Email and password are required");
        }

        logger.debug("Authenticator started", { email });

        // the type of this user must match the type you pass to the
        // Authenticator the strategy will automatically inherit the type if
        // you instantiate directly inside the `use` method
        return await AuthService.loginRequest({
            email: email,
            username_type: "email",
            password: password,
        }).then(response => response.json());
    }),
    
    // each strategy has a name and can be changed to use the same strategy
    // multiple times, especially useful for the OAuth2 strategy.
    "user-pass"
);
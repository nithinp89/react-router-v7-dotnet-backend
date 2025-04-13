import type { Config } from "@react-router/dev/config";
import "react-router";

declare module "react-router" {
  interface Future {
    unstable_middleware: false; // 👈 Enable middleware types
  }
}

export default {
  // Config options...
  // Server-side render by default, to enable SPA mode set this to `false`
  ssr: true,
  future: {
    unstable_middleware: false, // 👈 Enable middleware https://sergiodxa.com/tutorials/use-middleware-in-react-router
    // ...Other future or unstable flags
  },
} satisfies Config;

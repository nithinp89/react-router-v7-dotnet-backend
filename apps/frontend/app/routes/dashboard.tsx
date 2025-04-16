import type { Route } from "./+types/dashboard";
import { Welcome } from "../components/welcome/welcome";
import { useLoaderData, redirect } from "react-router";
import { AuthService } from "~/services/auth/auth.server";
import type { User } from "~/services/auth/types";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Dashboard" },
    { name: "description", content: "Welcome to Dashboard!" },
  ];
}

type DashboardLoaderData = {
  tenant: string;
  user: User | null; // TODO: Replace 'any' with your actual user type
};

export async function loader({ request }: Route.LoaderArgs): Promise<DashboardLoaderData | Response>  {
  const host = new URL(request.url).host;
  const tenant = host.split('.')[0] || '';
  console.log('🧾 [DASHBOARD] Tenant:', tenant);

  const userOrResponse  = await AuthService.getCurrentUserOrRedirect(request);
  // If it's a Response (redirect), return it directly
  if (userOrResponse instanceof Response) {
    return userOrResponse;
  }

  return ({tenant: tenant, user: userOrResponse });
}

export default function Dashboard() {
  const loaderData = useLoaderData<DashboardLoaderData>();
  return <Welcome tenant={loaderData?.tenant ?? null} user={loaderData?.user ?? null} />;
}

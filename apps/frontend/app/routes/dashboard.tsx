import type { Route } from "./+types/dashboard";
import { Welcome } from "../components/welcome/welcome";
import { useLoaderData } from "react-router";

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Dashboard" },
    { name: "description", content: "Welcome to Dashboard!" },
  ];
}

export async function loader({ request }: Route.LoaderArgs)  {
  const host = new URL(request.url).host;
  const tenant = host.split('.')[0] || '';
  console.log('🧾 [DASHBOARD] Tenant:', tenant);

  return { tenant: tenant };
}

export default function Dashboard({
  loaderData, 
}: Route.ComponentProps) {
  return <Welcome tenant={loaderData.tenant} />;
}

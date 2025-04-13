import { ArrowLeftIcon } from "lucide-react";
import { Link, Outlet } from "react-router";

export default function AuthLayout() {
    return (
        <div className="flex h-screen w-full items-center justify-center">
            <div className="mx-auto w-full ">
                <Outlet />
            </div>
        </div>
    );
}
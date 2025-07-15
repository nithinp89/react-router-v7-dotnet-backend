import React from "react";
import { cn } from "~/lib/utils"

type Props = {
  children: React.ReactNode;
  className?: string;
};

const Loader = ({ children, className = "" }: Props) => (
  <div className={cn("animate-pulse", className)} role="status">
    {children}
  </div>
);

type ItemProps = {
  height?: string;
  width?: string;
  className?: string;
};

const Item: React.FC<ItemProps> = ({ height = "auto", width = "auto", className = "" }) => (
  <div className={cn("rounded-md bg-custom-background-80", className)} style={{ height: height, width: width }} />
);

Loader.Item = Item;

Loader.displayName = "ui-loader";

export { Loader };

{/* <Loader className="space-y-10">
  <Loader.Item height="50px" width="75%" />
  <Loader.Item height="50px" width="75%" />
  <Loader.Item height="50px" width="40%" />
  <Loader.Item height="50px" width="40%" />
  <Loader.Item height="50px" width="20%" />
</Loader> */}
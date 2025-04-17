import { Toaster as Sonner, type ToasterProps } from "sonner"
import { useTheme } from "remix-themes"

const Toaster = ({ ...props }: ToasterProps) => {
  const theme: any = useTheme();

  return (
    <Sonner
      theme={theme.theme as ToasterProps["theme"]}
      className="toaster group"
      style={
        {
          "--normal-bg": "var(--popover)",
          "--normal-text": "var(--popover-foreground)",
          "--normal-border": "var(--border)",
        } as React.CSSProperties
      }
      {...props}
    />
  )
}

export { Toaster }

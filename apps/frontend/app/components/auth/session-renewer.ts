import { useEffect } from "react";
import { Routes } from "~/constants";
import { useAuthStore } from '~/stores/auth-store';

/**
 * Session Renewal Component to silently renew the jwt for active browser session
 * @param
 * @returns 
 */
export function SessionRenewal() {

  const expiry = useAuthStore((s: any) => s.refreshTokenExpiry);
  useEffect(() => {
    if (!expiry) return;
    const now = Math.floor(Date.now() / 1000);

    if (expiry > now) {
      const timer = setTimeout(() => {
        try {
          console.log('Session nenewal started:');
          
          fetch(Routes.AUTH_SESSION_RENEWER, { 
            method: "POST", 
            credentials: "include",
            headers: {
              "Accept": "application/json"
            }
          }).then(response => {
            if (!response.ok) {
              throw new Error('Failed to renew session');
            }
            return response.json();
          }).then(data => {
            console.log('Session renewed:', data);
            useAuthStore.setState({
              refreshTokenExpiry: data.refresh_token_expiry
            });
          });

        } catch (error) {
          console.log('Session renew error:', error);
        }
      }, (expiry - now) * 1000);

      return () => clearTimeout(timer);
    }
  }, [expiry]);

  return null;

}
import { useEffect } from "react";
import { Auth, Routes, Headers } from "~/constants";
import { useAuthStore } from '~/stores/auth-store';
import { v4 as uuidv4 } from 'uuid';
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
          fetch(Routes.AUTH_SESSION_RENEWER, { 
            method: "POST", 
            credentials: "include",
            headers: {
              [Headers.ACCEPT]: Headers.CONTENT_TYPE_JSON,
              [Headers.X_REQUEST_ID]: uuidv4(),
            }
          }).then(response => {
            if (!response.ok) {
              throw new Error(Auth.SESSION_RENEWAL_FAILED);
            }
            return response.json();
          }).then(data => {
            console.log(Auth.SESSION_RENEWAL_COMPLETED, data);
            if(data.refreshTokenExpiry > 0) {
              useAuthStore.setState({
                refreshTokenExpiry: data.refreshTokenExpiry
              });
            }
          });

        } catch (error) {
          console.log(Auth.SESSION_RENEWAL_ERROR, error);
        }
      }, (expiry - now) * 1000);

      return () => clearTimeout(timer);
    }
  }, [expiry]);

  return null;

}
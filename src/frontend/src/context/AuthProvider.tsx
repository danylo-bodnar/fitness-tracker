import { useCallback, useEffect, useState, type ReactNode } from "react";
import { toast } from "sonner";
import { AuthContext } from "./AuthContext";
import type { User } from "@/features/auth";
import { refreshAccessToken, tokenStore } from "@/lib/apiClient";
import { LoadingSpinner } from "@/components/feedback/Spinner";

const USER_KEY = "user";

function readStoredUser(): User | null {
  const saved = localStorage.getItem(USER_KEY);
  if (!saved) return null;

  try {
    return JSON.parse(saved) as User;
  } catch {
    // Corrupt or hand-edited entry — treat it as no session at all.
    localStorage.removeItem(USER_KEY);
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(readStoredUser);
  const [isInitializing, setIsInitializing] = useState(
    () => !!localStorage.getItem(USER_KEY),
  );

  const isAdmin = user?.role === "admin";

  const login = useCallback((newToken: string, newUser: User) => {
    tokenStore.set(newToken);
    localStorage.setItem(USER_KEY, JSON.stringify(newUser));
    setToken(newToken);
    setUser(newUser);
  }, []);

  const logout = useCallback((reason?: "expired") => {
    tokenStore.clear();
    localStorage.removeItem(USER_KEY);
    if (reason === "expired") {
      toast.error("Your session expired. Log in to continue.");
    }
    setToken(null);
    setUser(null);
  }, []);

  useEffect(() => {
    if (!localStorage.getItem(USER_KEY)) return;

    let ignore = false;

    refreshAccessToken()
      .then((accessToken) => {
        if (!ignore) setToken(accessToken);
      })
      .catch(() => {
        if (ignore) return;
        localStorage.removeItem(USER_KEY);
        setUser(null);
      })
      .finally(() => {
        if (!ignore) setIsInitializing(false);
      });

    return () => {
      // Stops a slow refresh from writing state after unmount or logout.
      ignore = true;
    };
  }, []);

  // Fired by the API client when a refresh fails on a 401.
  useEffect(() => {
    const handler = () => logout("expired");
    window.addEventListener("auth:logout", handler);
    return () => window.removeEventListener("auth:logout", handler);
  }, [logout]);

  if (isInitializing) return <LoadingSpinner />;

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token && !!user,
        isAdmin,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

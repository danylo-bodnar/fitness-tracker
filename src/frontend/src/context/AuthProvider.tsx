// context/AuthProvider.tsx
import axios from "axios";
import { useCallback, useEffect, useState, type ReactNode } from "react";
import { AuthContext } from "./AuthContext";
import type { User } from "@/features/auth";
import { tokenStore } from "@/lib/apiClient";
import { LoadingSpinner } from "@/components/feedback/Spinner";
import { toast } from "sonner";

const BASE_URL = import.meta.env.VITE_API_URL;

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(() => {
    const saved = localStorage.getItem("user");
    return saved ? JSON.parse(saved) : null;
  });
  const [isInitializing, setIsInitializing] = useState(
    () => !!localStorage.getItem("user"),
  );
  const isAdmin = user?.role === "admin";

  const login = useCallback((newToken: string, newUser: User) => {
    tokenStore.set(newToken);
    localStorage.setItem("user", JSON.stringify(newUser));
    setToken(newToken);
    setUser(newUser);
  }, []);

  const logout = useCallback(() => {
    tokenStore.clear();
    localStorage.removeItem("user");
    toast.error("Session expired. Please log in again.");
    setToken(null);
    setUser(null);
  }, []);

  useEffect(() => {
    if (!user) return;

    axios
      .post<{ accessToken: string }>(
        `${BASE_URL}/auth/refresh`,
        {},
        { withCredentials: true },
      )
      .then((res) => {
        tokenStore.set(res.data.accessToken);
        setToken(res.data.accessToken);
      })
      .catch(() => {
        localStorage.removeItem("user");
        setUser(null);
      })
      .finally(() => {
        setIsInitializing(false);
      });
  }, [user]);

  useEffect(() => {
    const handler = () => logout();
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

import { useCallback, useEffect, useState, type ReactNode } from "react";
import { AuthContext } from "./AuthContext";
import type { User } from "@/features/auth";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() =>
    localStorage.getItem("jwt-token"),
  );

  const [user, setUser] = useState<User | null>(() => {
    const item = localStorage.getItem("user");
    return item ? JSON.parse(item) : null;
  });

  const login = useCallback((newToken: string, newUser: User) => {
    localStorage.setItem("jwt-token", newToken);
    localStorage.setItem("user", JSON.stringify(newUser));

    setToken(newToken);
    setUser(newUser);
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem("jwt-token");
    localStorage.removeItem("user");

    setToken(null);
    setUser(null);
  }, []);

  useEffect(() => {
    const handler = () => {
      logout();
    };

    window.addEventListener("auth:logout", handler);

    return () => {
      window.removeEventListener("auth:logout", handler);
    };
  }, [logout]);

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token && !!user,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

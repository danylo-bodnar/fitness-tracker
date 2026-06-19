import { apiClient } from "@/lib/apiClient";
import type { User } from "../types";

export async function login(
  email: string,
  password: string,
): Promise<{ user: User; token: string }> {
  const res = await apiClient.post("/auth/login", { email, password });
  return res.data;
}

export async function logout(): Promise<void> {
  await apiClient.post("/auth/logout");
}

export async function getMe(): Promise<User> {
  const res = await apiClient.get("/auth/me");
  return res.data;
}

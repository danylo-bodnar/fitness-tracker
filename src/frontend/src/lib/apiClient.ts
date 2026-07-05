// lib/apiClient.ts
import axios from "axios";
import { toast } from "sonner";
import { getErrorMessage } from "./apiError";

let accessToken: string | null = null;

export const tokenStore = {
  get: () => accessToken,
  set: (token: string) => {
    accessToken = token;
  },
  clear: () => {
    accessToken = null;
  },
};

const BASE_URL = import.meta.env.VITE_API_URL;

export const apiClient = axios.create({
  baseURL: BASE_URL,
  withCredentials: true,
});

apiClient.interceptors.request.use((config) => {
  const token = tokenStore.get();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

let refreshPromise: Promise<string> | null = null;

async function refreshAccessToken(): Promise<string> {
  const res = await axios.post<{ accessToken: string }>(
    `${BASE_URL}/auth/refresh`,
    {},
    { withCredentials: true },
  );
  return res.data.accessToken;
}

apiClient.interceptors.response.use(
  (res) => res,
  async (error) => {
    const original = error.config;

    if (error.response?.status === 401 && !original._retry) {
      original._retry = true;

      try {
        refreshPromise ??= refreshAccessToken().finally(() => {
          refreshPromise = null;
        });

        const newToken = await refreshPromise;
        tokenStore.set(newToken);
        original.headers.Authorization = `Bearer ${newToken}`;
        return apiClient(original);
      } catch {
        tokenStore.clear();
        window.dispatchEvent(new Event("auth:logout"));
        return Promise.reject(error);
      }
    }

    if (!original._silentError) {
      toast.error(getErrorMessage(error));
    }

    return Promise.reject(error);
  },
);

import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { toast } from "sonner";
import { getErrorMessage } from "./apiError";

declare module "axios" {
  export interface InternalAxiosRequestConfig {
    /** Set once a request has already been retried after a token refresh. */
    _retry?: boolean;
    /** Opt out of the automatic error toast for this request. */
    _silentError?: boolean;
  }
}

const BASE_URL = import.meta.env.VITE_API_URL;

const REFRESH_PATH = "/auth/refresh";

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

export function refreshAccessToken(): Promise<string> {
  refreshPromise ??= axios
    .post<{ accessToken: string }>(
      `${BASE_URL}${REFRESH_PATH}`,
      {},
      { withCredentials: true },
    )
    .then((res) => {
      tokenStore.set(res.data.accessToken);
      return res.data.accessToken;
    })
    .finally(() => {
      refreshPromise = null;
    });

  return refreshPromise;
}

export function forceLogout() {
  tokenStore.clear();
  window.dispatchEvent(new Event("auth:logout"));
}

apiClient.interceptors.response.use(
  (res) => res,
  async (error: AxiosError) => {
    const original = error.config as InternalAxiosRequestConfig | undefined;

    // No config means the request never left the client (e.g. setup error) —
    // there is nothing to retry.
    if (!original) return Promise.reject(error);

    const isRefreshCall = original.url?.includes(REFRESH_PATH) ?? false;

    if (error.response?.status === 401 && !original._retry && !isRefreshCall) {
      original._retry = true;

      try {
        const newToken = await refreshAccessToken();
        original.headers.Authorization = `Bearer ${newToken}`;
        return await apiClient(original);
      } catch {
        forceLogout();
        return Promise.reject(error);
      }
    }

    if (!original._silentError) {
      toast.error(getErrorMessage(error));
    }

    return Promise.reject(error);
  },
);

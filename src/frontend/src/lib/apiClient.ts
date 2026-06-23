import axios from "axios";

export const apiClient = axios.create({
  baseURL: import.meta.env.DEV ? "" : import.meta.env.VITE_API_URL,
});

apiClient.interceptors.request.use((config) => {
  const jwt_token = localStorage.getItem("jwt-token");

  if (jwt_token) {
    config.headers.Authorization = `Bearer ${jwt_token}`;
  }

  return config;
});

apiClient.interceptors.response.use(
  (res) => res,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("jwt-token");

      window.dispatchEvent(new Event("auth:logout"));
    }

    return Promise.reject(error);
  },
);

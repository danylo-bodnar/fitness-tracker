// src/features/auth/hooks/useTelegramLoginStream.ts
import { useEffect, useRef } from "react";
import type { User } from "../types";

interface SseSuccessPayload {
  jwt: string;
  user: User;
}

export function useTelegramLoginStream(
  nonce: string | null,
  onSuccess: (jwt: string, user: User) => void,
) {
  const onSuccessRef = useRef(onSuccess);

  useEffect(() => {
    onSuccessRef.current = onSuccess;
  }, [onSuccess]);

  useEffect(() => {
    if (!nonce) return;

    const baseUrl = import.meta.env.DEV ? "" : import.meta.env.VITE_API_URL;
    const eventSource = new EventSource(`${baseUrl}/auth/stream/${nonce}`, {
      withCredentials: false,
    });

    eventSource.addEventListener("pending", () => {
      console.log("Waiting for Telegram approval...");
    });

    eventSource.addEventListener("success", (event) => {
      const data: SseSuccessPayload = JSON.parse(event.data);
      onSuccessRef.current(data.jwt, data.user);
      eventSource.close();
    });

    eventSource.addEventListener("expired", () => {
      console.warn("Login session expired");
      eventSource.close();
    });

    eventSource.addEventListener("login-error", (event) => {
      console.error("Login error:", event.data);
      eventSource.close();
    });

    eventSource.onerror = () => {
      console.error("SSE connection lost");
      eventSource.close();
    };

    return () => {
      eventSource.close();
    };
  }, [nonce]);
}

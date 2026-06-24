import { useEffect, useRef } from "react";
import type { User } from "../types";

interface SseSuccessPayload {
  accessToken: string;
  user: User;
  code: string;
}

export function useTelegramLoginStream(
  nonce: string | null,
  onSuccess: (accessToken: string, user: User) => void,
) {
  const onSuccessRef = useRef(onSuccess);
  useEffect(() => {
    onSuccessRef.current = onSuccess;
  }, [onSuccess]);

  useEffect(() => {
    if (!nonce) return;

    const baseUrl = import.meta.env.DEV ? "" : import.meta.env.VITE_API_URL;
    const eventSource = new EventSource(`${baseUrl}/auth/stream/${nonce}`, {
      withCredentials: true,
    });

    let completed = false;

    eventSource.addEventListener("pending", () => {
      console.log("Waiting for Telegram approval...");
    });

    eventSource.addEventListener("success", async (event) => {
      completed = true;
      const data: SseSuccessPayload = JSON.parse(event.data);
      await fetch(`${baseUrl}/auth/exchange`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ code: data.code }),
        credentials: "include",
      });
      onSuccessRef.current(data.accessToken, data.user);
      eventSource.close();
    });

    eventSource.addEventListener("expired", () => {
      completed = true;
      console.warn("Login session expired");
      eventSource.close();
    });

    eventSource.addEventListener("error", (event) => {
      completed = true;
      const messageEvent = event as MessageEvent;
      if (messageEvent.data) {
        console.error("Login error:", JSON.parse(messageEvent.data));
      }
      eventSource.close();
    });

    eventSource.onerror = () => {
      if (!completed) {
        console.error("SSE connection lost unexpectedly");
      }
    };

    return () => {
      completed = true;
      eventSource.close();
    };
  }, [nonce]);
}

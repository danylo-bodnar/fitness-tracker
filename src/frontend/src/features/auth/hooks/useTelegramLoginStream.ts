import { useEffect } from "react";

interface SseSuccessPayload {
  jwt: string;
  user: {
    id: string;
    telegramChatId: number;
    telegramUsername: string;
  };
}

export function useTelegramLoginStream(
  nonce: string | null,
  onSuccess: (jwt: string, user: SseSuccessPayload["user"]) => void,
) {
  useEffect(() => {
    if (!nonce) return;

    const baseUrl = import.meta.env.DEV ? "" : import.meta.env.VITE_API_URL;
    const eventSource = new EventSource(`${baseUrl}/auth/stream/${nonce}`, {
      withCredentials: false,
    });

    eventSource.addEventListener("pending", () => {
      console.log("waiting for approval...");
    });

    eventSource.addEventListener("success", (event) => {
      const data: SseSuccessPayload = JSON.parse(event.data);

      localStorage.setItem("jwt", data.jwt);
      localStorage.setItem("user", JSON.stringify(data.user));

      onSuccess(data.jwt, data.user);

      eventSource.close();
    });

    eventSource.addEventListener("expired", () => {
      console.log("login expired");
      eventSource.close();
    });

    eventSource.addEventListener("error", () => {
      console.log("SSE error");
      eventSource.close();
    });

    return () => {
      eventSource.close();
    };
  }, [nonce]);
}

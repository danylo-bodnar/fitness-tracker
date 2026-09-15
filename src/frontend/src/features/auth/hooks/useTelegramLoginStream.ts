import { useEffect, useRef, useState } from "react";
import type { User } from "../types";

interface SseSuccessPayload {
  accessToken: string;
  user: User;
  code: string;
}

export type TelegramLoginStatus =
  | "idle"
  | "waiting"
  | "exchanging"
  | "success"
  | "expired"
  | "error";

type StreamStatus = Exclude<TelegramLoginStatus, "idle" | "waiting">;

interface Options {
  onSuccess: (accessToken: string, user: User) => void;
  onError?: (message: string) => void;
}

export function useTelegramLoginStream(
  nonce: string | null,
  { onSuccess, onError }: Options,
) {
  const [streamStatus, setStreamStatus] = useState<StreamStatus | null>(null);

  const [trackedNonce, setTrackedNonce] = useState(nonce);
  if (trackedNonce !== nonce) {
    setTrackedNonce(nonce);
    setStreamStatus(null);
  }

  const status: TelegramLoginStatus =
    streamStatus ?? (nonce ? "waiting" : "idle");

  const onSuccessRef = useRef(onSuccess);
  const onErrorRef = useRef(onError);

  useEffect(() => {
    onSuccessRef.current = onSuccess;
  }, [onSuccess]);

  useEffect(() => {
    onErrorRef.current = onError;
  }, [onError]);

  useEffect(() => {
    if (!nonce) return;

    const baseUrl = import.meta.env.VITE_API_URL;
    const eventSource = new EventSource(`${baseUrl}/auth/stream/${nonce}`, {
      withCredentials: true,
    });

    // Guards against late handlers firing after the stream is done or the
    // component unmounted.
    let settled = false;

    const fail = (message: string) => {
      if (settled) return;
      settled = true;
      eventSource.close();
      setStreamStatus("error");
      onErrorRef.current?.(message);
    };

    eventSource.addEventListener("success", (event) => {
      if (settled) return;
      settled = true;
      eventSource.close();
      setStreamStatus("exchanging");

      // Kept as a separate async IIFE: throwing inside an async listener
      // produces an unhandled rejection that no one catches.
      void (async () => {
        try {
          const data = JSON.parse(
            (event as MessageEvent<string>).data,
          ) as SseSuccessPayload;

          const response = await fetch(`${baseUrl}/auth/exchange`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ code: data.code }),
            credentials: "include",
          });

          if (!response.ok) {
            throw new Error("Could not complete login. Try again.");
          }

          setStreamStatus("success");
          onSuccessRef.current(data.accessToken, data.user);
        } catch (err) {
          setStreamStatus("error");
          onErrorRef.current?.(
            err instanceof Error ? err.message : "Could not complete login.",
          );
        }
      })();
    });

    eventSource.addEventListener("expired", () => {
      if (settled) return;
      settled = true;
      eventSource.close();
      setStreamStatus("expired");
      onErrorRef.current?.("This login link expired. Request a new one.");
    });

    eventSource.addEventListener("error", (event) => {
      const messageEvent = event as MessageEvent<string | undefined>;

      if (messageEvent.data) {
        try {
          console.error("Telegram login error:", JSON.parse(messageEvent.data));
        } catch {
          console.error("Telegram login error:", messageEvent.data);
        }
        fail("Login failed. Try again.");
        return;
      }

      if (eventSource.readyState === EventSource.CLOSED) {
        fail("Lost connection during login. Try again.");
      }
    });

    return () => {
      settled = true;
      eventSource.close();
    };
  }, [nonce]);

  return { status };
}

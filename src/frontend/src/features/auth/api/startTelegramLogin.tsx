import { apiClient } from "@/lib/apiClient";

interface StartTelegramLoginResponse {
  nonce: string;
  telegramLink: string;
}

export async function startTelegramLogin() {
  const { data } = await apiClient.post<StartTelegramLoginResponse>(
    "/auth/start-telegram-login",
  );

  return data;
}

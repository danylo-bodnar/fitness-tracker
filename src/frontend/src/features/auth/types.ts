export interface User {
  id: string;
  telegramChatId: number;
  telegramUsername: string | null;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  status: "unauthenticated" | "pending_telegram" | "authenticated";
  telegramNonce: string | null;
}

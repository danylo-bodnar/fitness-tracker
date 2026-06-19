export interface User {
  telegramId: number;
  chatId: number;
  telegramUsername?: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  status: "unauthenticated" | "pending_telegram" | "authenticated";
  telegramNonce: string | null;
}

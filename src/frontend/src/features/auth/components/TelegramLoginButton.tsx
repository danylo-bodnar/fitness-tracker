import { useState } from "react";
import { Send } from "lucide-react";
import { Button } from "@/components/ui/button";
import { startTelegramLogin } from "@/features/auth/api/startTelegramLogin";
import { toast } from "sonner";

interface TelegramLoginButtonProps {
  onLoginStart?: (nonce: string) => void;
}

function TelegramLoginButton({ onLoginStart }: TelegramLoginButtonProps) {
  const [isLoading, setIsLoading] = useState(false);

  const handleLogin = async () => {
    try {
      setIsLoading(true);

      const { telegramLink, nonce } = await startTelegramLogin();

      localStorage.setItem("loginNonce", nonce);
      onLoginStart?.(nonce);

      window.open(telegramLink, "_blank", "noopener,noreferrer");
    } catch (error) {
      console.error(error);

      toast.error("Failed to start Telegram login");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Button
      className="w-full"
      size="lg"
      onClick={handleLogin}
      disabled={isLoading}
    >
      <Send className="size-4" />
      {isLoading ? "Opening Telegram..." : "Continue with Telegram"}
    </Button>
  );
}

export default TelegramLoginButton;

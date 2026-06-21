import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useTelegramLoginStream } from "@/features/auth/hooks/useTelegramLoginStream";
import { useAuthContext } from "@/context/AuthContext";
import TelegramLoginButton from "@/features/auth/components/TelegramLoginButton";

function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuthContext();
  const [nonce, setNonce] = useState<string | null>(null);

  useTelegramLoginStream(nonce, (jwt, user) => {
    login(jwt, user);
    localStorage.removeItem("loginNonce");
    navigate("/", { replace: true });
  });

  return (
    <div className="flex min-h-screen items-center justify-center px-4">
      <div className="w-full max-w-sm space-y-6 rounded-lg border p-6 shadow-sm">
        <div className="space-y-2 text-center">
          <h1 className="text-2xl font-semibold">Sign in</h1>

          <p className="text-sm text-muted-foreground">
            Login securely using Telegram.
          </p>
        </div>

        <TelegramLoginButton onLoginStart={(n) => setNonce(n)} />
      </div>
    </div>
  );
}

export default LoginPage;

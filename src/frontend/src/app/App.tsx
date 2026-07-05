import { Toaster } from "sonner";
import { QueryClientProvider } from "@tanstack/react-query";
import { queryClient } from "@/lib/queryClient";
import AppRouter from "@/app/AppRouter";
import ErrorBoundary from "@/components/feedback/ErrorBoundary";
import { AuthProvider } from "@/context/AuthProvider";

function App() {
  return (
    <ErrorBoundary>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <AppRouter />
        </AuthProvider>
      </QueryClientProvider>
      <Toaster richColors />
    </ErrorBoundary>
  );
}

export default App;

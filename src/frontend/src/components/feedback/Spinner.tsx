import { Spinner } from "@/components/ui/spinner";

export function LoadingSpinner() {
  return (
    <div className="flex items-center justify-center p-6">
      <Spinner className="size-8" />
    </div>
  );
}

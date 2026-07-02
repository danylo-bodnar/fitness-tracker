import { Dumbbell } from "lucide-react";

export function EmptyPrograms() {
  return (
    <div className="flex flex-col items-center justify-center rounded-xl border border-dashed py-16 text-center">
      <Dumbbell className="mb-3 size-8 text-muted-foreground" />
      <p className="font-medium">No programs yet</p>
      <p className="mt-1 text-sm text-muted-foreground">
        Create your first training program to get started.
      </p>
    </div>
  );
}

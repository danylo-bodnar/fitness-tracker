import { useState } from "react";
import { useExercises } from "@/features/exercises";
import { Input } from "@/components/ui/input";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import { Search } from "lucide-react";

interface AddExerciseDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSelect: (exerciseId: string, exerciseName: string) => void;
  excludeIds: string[];
}

export function AddExerciseDialog({
  open,
  onOpenChange,
  onSelect,
  excludeIds,
}: AddExerciseDialogProps) {
  const [search, setSearch] = useState("");
  const { data: exercises, isLoading } = useExercises();

  const filtered = (exercises ?? []).filter(
    (ex) =>
      !excludeIds.includes(ex.id) &&
      ex.name.toLowerCase().includes(search.toLowerCase()),
  );

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Add Exercise</DialogTitle>
          <DialogDescription>
            Search and select an exercise to add to this day.
          </DialogDescription>
        </DialogHeader>
        <div className="relative">
          <Search className="absolute left-2.5 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search exercises..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="pl-8"
            autoFocus
          />
        </div>
        <div className="max-h-60 space-y-1 overflow-y-auto">
          {isLoading ? (
            <p className="py-4 text-center text-sm text-muted-foreground">
              Loading exercises...
            </p>
          ) : filtered.length === 0 ? (
            <p className="py-4 text-center text-sm text-muted-foreground">
              No exercises found.
            </p>
          ) : (
            filtered.map((ex) => (
              <button
                key={ex.id}
                className="flex w-full items-center justify-between rounded-md px-3 py-2 text-left text-sm hover:bg-muted transition-colors"
                onClick={() => {
                  onSelect(ex.id, ex.name);
                  onOpenChange(false);
                  setSearch("");
                }}
              >
                <span className="capitalize">{ex.name}</span>
                {ex.muscleGroup && (
                  <span className="text-xs text-muted-foreground">
                    {ex.muscleGroup}
                  </span>
                )}
              </button>
            ))
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}

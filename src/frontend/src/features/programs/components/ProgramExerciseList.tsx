import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { X } from "lucide-react";
import type { ProgramExerciseDto } from "../types";

interface ProgramExerciseListProps {
  exercises: ProgramExerciseDto[];
  editing?: boolean;
  onUpdate?: (index: number, exercise: ProgramExerciseDto) => void;
  onRemove?: (index: number) => void;
}

export function ProgramExerciseList({
  exercises,
  editing = false,
  onUpdate,
  onRemove,
}: ProgramExerciseListProps) {
  if (editing) {
    return (
      <div className="mt-2 space-y-1.5">
        {exercises.map((ex, i) => (
          <div
            key={ex.exerciseId}
            className="flex items-center gap-2 rounded-md border px-2.5 py-1.5 text-sm"
          >
            <span className="w-4 text-right text-xs text-muted-foreground shrink-0">
              {ex.order}.
            </span>
            <span className="flex-1 truncate capitalize">{ex.exerciseName}</span>
            <div className="flex items-center gap-1 shrink-0">
              <Input
                type="number"
                min={1}
                value={ex.targetSets}
                onChange={(e) =>
                  onUpdate?.(i, {
                    ...ex,
                    targetSets: Number(e.target.value),
                  })
                }
                className="h-7 w-12 text-center text-xs [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
              />
              <span className="text-xs text-muted-foreground">×</span>
              <Input
                type="number"
                min={1}
                value={ex.targetReps}
                onChange={(e) =>
                  onUpdate?.(i, {
                    ...ex,
                    targetReps: Number(e.target.value),
                  })
                }
                className="h-7 w-12 text-center text-xs [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
              />
            </div>
            <Button
              variant="ghost"
              size="icon-xs"
              className="size-6 shrink-0 text-muted-foreground hover:text-destructive"
              onClick={() => onRemove?.(i)}
            >
              <X className="size-3" />
            </Button>
          </div>
        ))}
      </div>
    );
  }

  return (
    <ul className="mt-2 space-y-1">
      {exercises.map((ex) => (
        <li
          key={ex.exerciseId}
          className="flex items-center gap-3 text-sm"
        >
          <span className="w-4 text-right text-xs text-muted-foreground">
            {ex.order}.
          </span>
          <span className="flex-1 capitalize">{ex.exerciseName}</span>
          <span className="text-xs text-muted-foreground">
            {ex.targetSets} × {ex.targetReps}
          </span>
        </li>
      ))}
    </ul>
  );
}

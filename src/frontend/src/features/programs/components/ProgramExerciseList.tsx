import type { ProgramExerciseDto } from "../types";

interface ProgramExerciseListProps {
  exercises: ProgramExerciseDto[];
}

export function ProgramExerciseList({ exercises }: ProgramExerciseListProps) {
  return (
    <ul className="mt-2 space-y-1">
      {exercises.map((ex) => (
        <li key={ex.exerciseId} className="flex items-center gap-3 text-sm">
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

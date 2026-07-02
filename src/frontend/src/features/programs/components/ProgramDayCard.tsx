import { useState } from "react";
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { ChevronDown, Plus, Trash2 } from "lucide-react";
import type { ProgramDayDto, ProgramExerciseDto } from "../types";
import { ProgramExerciseList } from "./ProgramExerciseList";
import { AddExerciseDialog } from "./AddExerciseDialog";

interface ProgramDayCardProps {
  day: ProgramDayDto;
  editing?: boolean;
  onUpdate?: (day: ProgramDayDto) => void;
  onRemove?: () => void;
}

export function ProgramDayCard({
  day,
  editing = false,
  onUpdate,
  onRemove,
}: ProgramDayCardProps) {
  const [open, setOpen] = useState(false);
  const [addExerciseOpen, setAddExerciseOpen] = useState(false);

  const handleExerciseUpdate = (index: number, exercise: ProgramExerciseDto) => {
    const exercises = day.exercises.map((ex, i) => (i === index ? exercise : ex));
    onUpdate?.({ ...day, exercises });
  };

  const handleExerciseRemove = (index: number) => {
    const exercises = day.exercises
      .filter((_, i) => i !== index)
      .map((ex, i) => ({ ...ex, order: i + 1 }));
    onUpdate?.({ ...day, exercises });
  };

  const handleAddExercise = (exerciseId: string, exerciseName: string) => {
    const newExercise: ProgramExerciseDto = {
      exerciseId,
      exerciseName,
      targetSets: 3,
      targetReps: 10,
      order: day.exercises.length + 1,
    };
    onUpdate?.({ ...day, exercises: [...day.exercises, newExercise] });
  };

  return (
    <>
      <Collapsible open={open} onOpenChange={setOpen}>
        <CollapsibleTrigger className="flex w-full items-center justify-between rounded-md px-3 py-2 text-sm font-medium hover:bg-muted/50 transition-colors">
          {editing ? (
            <Input
              value={day.name}
              onChange={(e) => onUpdate?.({ ...day, name: e.target.value })}
              className="h-6 text-sm font-medium"
              onClick={(e) => e.stopPropagation()}
            />
          ) : (
            <span>{day.name}</span>
          )}
          <div className="flex items-center gap-2">
            {editing && (
              <Button
                variant="ghost"
                size="icon-xs"
                className="size-6 text-muted-foreground hover:text-destructive"
                onClick={(e) => {
                  e.stopPropagation();
                  onRemove?.();
                }}
              >
                <Trash2 className="size-3" />
              </Button>
            )}
            <span className="text-xs text-muted-foreground">
              {day.exercises.length} exercise
              {day.exercises.length !== 1 ? "s" : ""}
            </span>
            <ChevronDown
              className={`size-4 text-muted-foreground transition-transform ${open ? "rotate-180" : ""}`}
            />
          </div>
        </CollapsibleTrigger>
        <CollapsibleContent className="px-3 pb-2">
          <ProgramExerciseList
            exercises={day.exercises}
            editing={editing}
            onUpdate={handleExerciseUpdate}
            onRemove={handleExerciseRemove}
          />
          {editing && (
            <Button
              variant="ghost"
              size="sm"
              className="mt-1.5 w-full text-xs"
              onClick={() => setAddExerciseOpen(true)}
            >
              <Plus className="size-3" />
              Add Exercise
            </Button>
          )}
        </CollapsibleContent>
      </Collapsible>
      <AddExerciseDialog
        open={addExerciseOpen}
        onOpenChange={setAddExerciseOpen}
        onSelect={handleAddExercise}
        excludeIds={day.exercises.map((ex) => ex.exerciseId)}
      />
    </>
  );
}

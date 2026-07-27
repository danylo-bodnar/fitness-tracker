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
      id: crypto.randomUUID(),
      exerciseId,
      exerciseName,
      targetSets: 3,
      targetReps: 10,
      order: day.exercises.length + 1,
    };
    onUpdate?.({ ...day, exercises: [...day.exercises, newExercise] });
  };

  const handleDragDrop = (dragIndex: number, dropIndex: number) => {
    const exercises = day.exercises.map(ex => ({ ...ex }));
    const dragged = exercises[dragIndex];
    const dropped = exercises[dropIndex];

    if (dropped.supersetGroupId != null) {
      exercises[dragIndex] = { ...dragged, supersetGroupId: dropped.supersetGroupId };
    } else {
      const used = exercises
        .map(e => e.supersetGroupId)
        .filter((id): id is number => id != null);
      const nextId = used.length > 0 ? Math.max(...used) + 1 : 1;
      exercises[dragIndex] = { ...dragged, supersetGroupId: nextId };
      exercises[dropIndex] = { ...dropped, supersetGroupId: nextId };
    }

    onUpdate?.({ ...day, exercises });
  };

  const handleUngroup = (index: number) => {
    const exercises = day.exercises.map((ex, i) =>
      i === index ? { ...ex, supersetGroupId: null } : ex
    );
    onUpdate?.({ ...day, exercises });
  };

  return (
    <>
      <Collapsible open={open} onOpenChange={setOpen}>
        <div className="flex items-center gap-1">
          <CollapsibleTrigger className="flex flex-1 items-center justify-between rounded-md px-3 py-2 text-sm font-medium hover:bg-muted/50 transition-colors">
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
              <span className="text-xs text-muted-foreground">
                {day.exercises.length} exercise
                {day.exercises.length !== 1 ? "s" : ""}
              </span>
              <ChevronDown
                className={`size-4 text-muted-foreground transition-transform ${open ? "rotate-180" : ""}`}
              />
            </div>
          </CollapsibleTrigger>
          {editing && (
            <Button
              variant="ghost"
              size="icon-xs"
              className="mr-1 size-6 shrink-0 text-muted-foreground hover:text-destructive"
              onClick={(e) => {
                e.stopPropagation();
                onRemove?.();
              }}
            >
              <Trash2 className="size-3" />
            </Button>
          )}
        </div>
        <CollapsibleContent className="px-3 pb-2">
          <ProgramExerciseList
            exercises={day.exercises}
            editing={editing}
            onUpdate={handleExerciseUpdate}
            onRemove={handleExerciseRemove}
            onDragDrop={handleDragDrop}
            onUngroup={handleUngroup}
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

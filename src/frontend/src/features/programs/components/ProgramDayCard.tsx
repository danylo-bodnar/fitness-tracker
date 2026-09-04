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
import { ProgramExerciseList, buildList } from "./ProgramExerciseList";
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

  const handleDragDrop = (dragEntryIndex: number, dropEntryIndex: number) => {
    if (dragEntryIndex === dropEntryIndex) return;

    const entries = buildList(day.exercises);
    // Drop at end (after last entry) is allowed: dropEntryIndex === entries.length
    const isDropAtEnd = dropEntryIndex === entries.length;
    if (dragEntryIndex < 0 || dragEntryIndex >= entries.length) return;
    if (!isDropAtEnd && (dropEntryIndex < 0 || dropEntryIndex >= entries.length)) return;

    const srcEntry = entries[dragEntryIndex];
    const dstEntry = isDropAtEnd ? null : entries[dropEntryIndex];

    const srcStart = srcEntry.type === "standalone" ? srcEntry.flatIndex : srcEntry.firstIndex;
    const srcLen = srcEntry.type === "standalone" ? 1 : srcEntry.exercises.length;

    let dstStart: number;
    let dstLen = 0;
    if (isDropAtEnd) {
      dstStart = day.exercises.length;
    } else if (dstEntry) {
      dstStart = dstEntry.type === "standalone" ? dstEntry.flatIndex : dstEntry.firstIndex;
      dstLen = dstEntry.type === "standalone" ? 1 : dstEntry.exercises.length;
    } else {
      dstStart = day.exercises.length;
    }

    // Adjust destination if source is before destination
    if (srcStart < dstStart) {
      // Moving forward: insert after destination block (intuitive)
      // For drop at end, just adjust for removed block
      if (isDropAtEnd) {
        dstStart -= srcLen;
      } else {
        dstStart = dstStart - srcLen + dstLen;
      }
    }

    const flat = [...day.exercises];
    const block = flat.splice(srcStart, srcLen);
    flat.splice(dstStart, 0, ...block);

    const reordered = flat.map((ex, i) => ({ ...ex, order: i + 1 }));
    onUpdate?.({ ...day, exercises: reordered });
  };

  const handleGroup = (flatIndex: number) => {
    if (flatIndex < 0 || flatIndex >= day.exercises.length - 1) return;
    const exercises = day.exercises.map((e) => ({ ...e }));
    const a = exercises[flatIndex];
    const b = exercises[flatIndex + 1];

    if (a.supersetGroupId != null && a.supersetGroupId === b.supersetGroupId) return;

    let targetId: number;
    if (a.supersetGroupId != null) targetId = a.supersetGroupId;
    else if (b.supersetGroupId != null) targetId = b.supersetGroupId;
    else {
      const used = exercises
        .map((e) => e.supersetGroupId)
        .filter((id): id is number => id != null);
      targetId = used.length > 0 ? Math.max(...used) + 1 : 1;
    }

    exercises[flatIndex] = { ...a, supersetGroupId: targetId };
    exercises[flatIndex + 1] = { ...b, supersetGroupId: targetId };

    onUpdate?.({ ...day, exercises });
  };

  const handleUngroup = (index: number) => {
    const target = day.exercises[index];
    if (!target) return;
    const groupId = target.supersetGroupId;
    let exercises = day.exercises.map((ex, i) =>
      i === index ? { ...ex, supersetGroupId: null } : ex
    );

    // If group now has 0 or 1 member, clear orphan
    if (groupId != null) {
      const remaining = exercises.filter((e) => e.supersetGroupId === groupId);
      if (remaining.length <= 1) {
        exercises = exercises.map((e) =>
          e.supersetGroupId === groupId ? { ...e, supersetGroupId: null } : e
        );
      }
    }

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
            onGroup={handleGroup}
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

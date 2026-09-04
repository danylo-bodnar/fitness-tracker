import { useState } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { GripVertical, X, Unlink2, Link2 } from "lucide-react";
import type { ProgramExerciseDto } from "../types";

const GROUP_COLORS = [
  "border-l-purple-500",
  "border-l-blue-500",
  "border-l-emerald-500",
  "border-l-amber-500",
  "border-l-rose-500",
];

type ListEntry =
  | { type: "standalone"; exercise: ProgramExerciseDto; flatIndex: number }
  | { type: "superset"; exercises: ProgramExerciseDto[]; groupId: number; firstIndex: number };

// eslint-disable-next-line react-refresh/only-export-components
export function buildList(exercises: ProgramExerciseDto[]): ListEntry[] {
  const entries: ListEntry[] = [];
  let i = 0;
  let flatIdx = 0;

  while (i < exercises.length) {
    const ex = exercises[i];
    if (ex.supersetGroupId != null) {
      const groupId = ex.supersetGroupId;
      const group: ProgramExerciseDto[] = [];
      let j = i;
      while (j < exercises.length && exercises[j].supersetGroupId === groupId) {
        group.push(exercises[j]);
        j++;
      }
      if (group.length > 1) {
        entries.push({ type: "superset", exercises: group, groupId, firstIndex: flatIdx });
        flatIdx += group.length;
        i = j;
        continue;
      }
      // single item with supersetId -> treat as standalone (orphan)
    }
    entries.push({ type: "standalone", exercise: ex, flatIndex: flatIdx });
    flatIdx++;
    i++;
  }

  return entries;
}

function getGroupColor(groupId: number, exercises: ProgramExerciseDto[]) {
  const used = [...new Set(exercises.map(e => e.supersetGroupId).filter(id => id != null))];
  const idx = used.indexOf(groupId);
  return GROUP_COLORS[idx % GROUP_COLORS.length];
}

interface ProgramExerciseListProps {
  exercises: ProgramExerciseDto[];
  editing?: boolean;
  onUpdate?: (index: number, exercise: ProgramExerciseDto) => void;
  onRemove?: (index: number) => void;
  onDragDrop?: (dragEntryIndex: number, dropEntryIndex: number) => void;
  onGroup?: (index: number) => void;
  onUngroup?: (index: number) => void;
}

export function ProgramExerciseList({
  exercises,
  editing = false,
  onUpdate,
  onRemove,
  onDragDrop,
  onGroup,
  onUngroup,
}: ProgramExerciseListProps) {
  const [dragOverEntry, setDragOverEntry] = useState<number | null>(null);
  const [dragSourceEntry, setDragSourceEntry] = useState<number | null>(null);

  const entries = buildList(exercises);

  if (editing) {
    return (
      <div className="mt-2 space-y-1.5">
        {entries.map((entry, entryIndex) => {
          const isDragOver = dragOverEntry === entryIndex;
          const isDragging = dragSourceEntry === entryIndex;

          if (entry.type === "standalone") {
            const ex = entry.exercise;
            const canGroup = entry.flatIndex < exercises.length - 1;
            return (
              <div
                key={ex.id}
                draggable
                onDragStart={(e) => {
                  setDragSourceEntry(entryIndex);
                  e.dataTransfer.effectAllowed = "move";
                  e.dataTransfer.setData("text/plain", String(entryIndex));
                }}
                onDragOver={(e) => { e.preventDefault(); e.dataTransfer.dropEffect = "move"; setDragOverEntry(entryIndex); }}
                onDragLeave={() => setDragOverEntry(null)}
                onDrop={(e) => {
                  e.preventDefault();
                  setDragOverEntry(null);
                  const src = dragSourceEntry;
                  setDragSourceEntry(null);
                  if (src != null && src !== entryIndex) {
                    onDragDrop?.(src, entryIndex);
                  }
                }}
                onDragEnd={() => { setDragOverEntry(null); setDragSourceEntry(null); }}
                className={`flex items-center gap-2 rounded-md border px-2.5 py-1.5 text-sm cursor-default transition-all ${isDragOver ? "ring-2 ring-primary/40 bg-muted/30" : ""} ${isDragging ? "opacity-50" : ""}`}
              >
                <span className="cursor-grab active:cursor-grabbing text-muted-foreground shrink-0">
                  <GripVertical className="size-4" />
                </span>
                <span className="w-4 text-right text-xs text-muted-foreground shrink-0">
                  {ex.order}.
                </span>
                <span className="flex-1 truncate capitalize">{ex.exerciseName}</span>
                <div className="flex items-center gap-1 shrink-0">
                  <Input
                    type="number"
                    min={1}
                    value={ex.targetSets}
                    onChange={(e) => onUpdate?.(entry.flatIndex, { ...ex, targetSets: Number(e.target.value) })}
                    className="h-7 w-10 text-center text-xs [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
                  />
                  <span className="text-xs text-muted-foreground">×</span>
                  <Input
                    type="number"
                    min={1}
                    value={ex.targetReps}
                    onChange={(e) => onUpdate?.(entry.flatIndex, { ...ex, targetReps: Number(e.target.value) })}
                    className="h-7 w-10 text-center text-xs [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
                  />
                </div>
                {onGroup && (
                  <Button
                    variant="ghost"
                    size="icon-xs"
                    className="size-6 shrink-0 text-muted-foreground hover:text-orange-500"
                    onClick={() => onGroup(entry.flatIndex)}
                    disabled={!canGroup}
                    title={canGroup ? "Group with next exercise (superset)" : "No next exercise to group"}
                  >
                    <Link2 className="size-3" />
                  </Button>
                )}
                <Button
                  variant="ghost"
                  size="icon-xs"
                  className="size-6 shrink-0 text-muted-foreground hover:text-destructive"
                  onClick={() => onRemove?.(entry.flatIndex)}
                >
                  <X className="size-3" />
                </Button>
              </div>
            );
          }

          // Superset group
          const group = entry.exercises;
          const color = getGroupColor(entry.groupId, exercises);
          const isDraggingGroup = dragSourceEntry === entryIndex;

          return (
            <div
              key={`group-${entry.groupId}-${entry.firstIndex}`}
              draggable
              onDragStart={(e) => {
                setDragSourceEntry(entryIndex);
                e.dataTransfer.effectAllowed = "move";
                e.dataTransfer.setData("text/plain", String(entryIndex));
              }}
              onDragOver={(e) => { e.preventDefault(); e.dataTransfer.dropEffect = "move"; setDragOverEntry(entryIndex); }}
              onDragLeave={() => setDragOverEntry(null)}
              onDrop={(e) => {
                e.preventDefault();
                setDragOverEntry(null);
                const src = dragSourceEntry;
                setDragSourceEntry(null);
                if (src != null && src !== entryIndex) {
                  onDragDrop?.(src, entryIndex);
                }
              }}
              onDragEnd={() => { setDragOverEntry(null); setDragSourceEntry(null); }}
              className={`rounded-md border border-l-[3px] ${color} text-sm cursor-default transition-all ${isDragOver ? "ring-2 ring-primary/40 bg-muted/30" : ""} ${isDraggingGroup ? "opacity-50" : ""}`}
            >
              {/* Group header */}
              <div className="flex items-center gap-2 px-2.5 py-1.5 bg-muted/20">
                <span className="cursor-grab active:cursor-grabbing text-muted-foreground shrink-0">
                  <GripVertical className="size-4" />
                </span>
                <span className="w-4 text-right text-xs text-muted-foreground shrink-0">
                  {group[0].order}.
                </span>
                <span className="flex-1 truncate text-xs font-medium text-orange-600 flex items-center gap-1">
                  <Link2 className="size-3" />
                  Superset: {group.map(e => e.exerciseName).join(" + ")}
                </span>
                <Button
                  variant="ghost"
                  size="icon-xs"
                  className="size-6 shrink-0 text-muted-foreground hover:text-destructive"
                  onClick={() => onRemove?.(entry.firstIndex)}
                >
                  <X className="size-3" />
                </Button>
              </div>

              {/* Individual exercises within the group */}
              <div className="divide-y">
                {group.map((ex, gi) => {
                  const flatIdx = entry.firstIndex + gi;
                  return (
                    <div key={ex.exerciseId} className="flex items-center gap-2 px-2.5 py-1.5 text-sm">
                      <span className="w-[18px] shrink-0" />
                      <span className="w-4 text-right text-xs text-muted-foreground shrink-0">
                        {ex.order}.
                      </span>
                      <span className="flex-1 truncate capitalize text-muted-foreground">
                        {ex.exerciseName}
                      </span>
                      <div className="flex items-center gap-1 shrink-0">
                        <Input
                          type="number"
                          min={1}
                          value={ex.targetSets}
                          onChange={(e) => onUpdate?.(flatIdx, { ...ex, targetSets: Number(e.target.value) })}
                          className="h-7 w-10 text-center text-xs [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
                        />
                        <span className="text-xs text-muted-foreground">×</span>
                        <Input
                          type="number"
                          min={1}
                          value={ex.targetReps}
                          onChange={(e) => onUpdate?.(flatIdx, { ...ex, targetReps: Number(e.target.value) })}
                          className="h-7 w-10 text-center text-xs [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
                        />
                      </div>
                      <button
                        onClick={() => onUngroup?.(flatIdx)}
                        className="text-xs text-muted-foreground hover:text-orange-500 shrink-0"
                        title="Remove from superset"
                      >
                        <Unlink2 className="size-3" />
                      </button>
                    </div>
                  );
                })}
              </div>
            </div>
          );
        })}
        {/* Drop zone at end to allow moving to last position */}
        {entries.length > 0 && (
          <div
            onDragOver={(e) => { e.preventDefault(); e.dataTransfer.dropEffect = "move"; setDragOverEntry(entries.length); }}
            onDragLeave={() => setDragOverEntry(null)}
            onDrop={(e) => {
              e.preventDefault();
              setDragOverEntry(null);
              const src = dragSourceEntry;
              setDragSourceEntry(null);
              if (src != null && src !== entries.length) {
                // Insert after last entry: treat drop index as entries.length
                onDragDrop?.(src, entries.length);
              }
            }}
            className={`h-2 rounded transition-all ${dragOverEntry === entries.length ? "bg-primary/20 ring-2 ring-primary/40 h-8" : ""}`}
          />
        )}
      </div>
    );
  }

  // View mode
  return (
    <ul className="mt-2 space-y-1">
      {entries.map((entry) => {
        if (entry.type === "standalone") {
          const ex = entry.exercise;
          return (
            <li key={ex.exerciseId} className="flex items-center gap-3 text-sm">
              <span className="w-4 text-right text-xs text-muted-foreground shrink-0">
                {ex.order}.
              </span>
              <span className="flex-1 capitalize">{ex.exerciseName}</span>
              <span className="text-xs text-muted-foreground">
                {ex.targetSets} × {ex.targetReps}
              </span>
            </li>
          );
        }

        const group = entry.exercises;
        const color = getGroupColor(entry.groupId, exercises);
        const name = group.map(e => e.exerciseName).join(" + ");
        const stats = group.map(e => `${e.targetSets}×${e.targetReps}`).join(" / ");

        return (
          <li
            key={`group-${entry.groupId}`}
            className={`flex items-center gap-3 text-sm border-l-[3px] pl-2 ${color}`}
          >
            <span className="w-4 text-right text-xs text-muted-foreground shrink-0">
              {group[0].order}.
            </span>
            <span className="flex-1 truncate capitalize flex items-center gap-1">
              <Link2 className="size-3 text-orange-500" />
              {name}
            </span>
            <span className="text-xs text-muted-foreground">{stats}</span>
          </li>
        );
      })}
    </ul>
  );
}

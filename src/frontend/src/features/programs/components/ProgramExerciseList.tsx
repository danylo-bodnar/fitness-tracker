import { useRef, useState } from "react";
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

function buildList(exercises: ProgramExerciseDto[]): ListEntry[] {
  const entries: ListEntry[] = [];
  const used = new Set<number>();
  let flatIdx = 0;

  for (let i = 0; i < exercises.length; i++) {
    const ex = exercises[i];
    if (used.has(i)) continue;

    if (ex.supersetGroupId != null) {
      const group = exercises.filter((e, idx) => {
        if (e.supersetGroupId === ex.supersetGroupId && !used.has(idx)) {
          used.add(idx);
          return true;
        }
        return false;
      });
      entries.push({ type: "superset", exercises: group, groupId: ex.supersetGroupId, firstIndex: flatIdx });
      flatIdx += group.length;
    } else {
      entries.push({ type: "standalone", exercise: ex, flatIndex: flatIdx });
      flatIdx++;
    }
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
  onDragDrop?: (dragIndex: number, dropIndex: number) => void;
  onUngroup?: (index: number) => void;
}

export function ProgramExerciseList({
  exercises,
  editing = false,
  onUpdate,
  onRemove,
  onDragDrop,
  onUngroup,
}: ProgramExerciseListProps) {
  const [dragOverEntry, setDragOverEntry] = useState<number | null>(null);
  const dragSourceEntry = useRef<number | null>(null);

  const entries = buildList(exercises);

  if (editing) {
    return (
      <div className="mt-2 space-y-1.5">
        {entries.map((entry, entryIndex) => {
          const isDragOver = dragOverEntry === entryIndex;

          if (entry.type === "standalone") {
            const ex = entry.exercise;
            return (
              <div
                key={ex.exerciseId}
                draggable
                onDragStart={() => { dragSourceEntry.current = entryIndex; }}
                onDragOver={(e) => { e.preventDefault(); setDragOverEntry(entryIndex); }}
                onDragLeave={() => setDragOverEntry(null)}
                onDrop={() => {
                  setDragOverEntry(null);
                  const src = dragSourceEntry.current;
                  dragSourceEntry.current = null;
                  if (src != null && src !== entryIndex) {
                    const srcEntry = entries[src];
                    const srcFlat = srcEntry.type === "standalone" ? srcEntry.flatIndex : srcEntry.firstIndex;
                    onDragDrop?.(srcFlat, entry.flatIndex);
                  }
                }}
                onDragEnd={() => { setDragOverEntry(null); dragSourceEntry.current = null; }}
                className={`flex items-center gap-2 rounded-md border px-2.5 py-1.5 text-sm cursor-default transition-all ${isDragOver ? "ring-2 ring-primary/40" : ""}`}
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

          return (
            <div
              key={`group-${entry.groupId}`}
              draggable
              onDragStart={() => { dragSourceEntry.current = entryIndex; }}
              onDragOver={(e) => { e.preventDefault(); setDragOverEntry(entryIndex); }}
              onDragLeave={() => setDragOverEntry(null)}
              onDrop={() => {
                setDragOverEntry(null);
                const src = dragSourceEntry.current;
                dragSourceEntry.current = null;
                if (src != null && src !== entryIndex) {
                  const srcEntry = entries[src];
                  const srcFlat = srcEntry.type === "standalone" ? srcEntry.flatIndex : srcEntry.firstIndex;
                  onDragDrop?.(srcFlat, entry.firstIndex);
                }
              }}
              onDragEnd={() => { setDragOverEntry(null); dragSourceEntry.current = null; }}
              className={`rounded-md border border-l-[3px] ${color} text-sm cursor-default transition-all ${isDragOver ? "ring-2 ring-primary/40" : ""}`}
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

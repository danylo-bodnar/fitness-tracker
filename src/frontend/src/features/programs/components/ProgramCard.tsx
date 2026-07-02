import { useState } from "react";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Trash2, Pencil, Save, X, Plus } from "lucide-react";
import type { WorkoutProgramDto, ProgramDayDto } from "../types";
import { ProgramDayCard } from "./ProgramDayCard";
import { AddDayDialog } from "./AddDayDialog";

interface ProgramCardProps {
  program: WorkoutProgramDto;
  onDelete: (id: string) => void;
  onUpdate?: (id: string, data: { name: string; programDays: ProgramDayDto[] }) => void;
  isDeleting?: boolean;
  isUpdating?: boolean;
}

export function ProgramCard({
  program,
  onDelete,
  onUpdate,
  isDeleting,
  isUpdating,
}: ProgramCardProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [draft, setDraft] = useState<WorkoutProgramDto>(() =>
    JSON.parse(JSON.stringify(program)),
  );
  const [addDayOpen, setAddDayOpen] = useState(false);

  const handleStartEdit = () => {
    setDraft(JSON.parse(JSON.stringify(program)));
    setIsEditing(true);
  };

  const handleCancel = () => {
    setIsEditing(false);
  };

  const handleSave = () => {
    onUpdate?.(program.id, {
      name: draft.name,
      programDays: draft.days,
    });
    setIsEditing(false);
  };

  const handleDayUpdate = (index: number, updated: ProgramDayDto) => {
    setDraft((prev) => {
      const days = [...prev.days];
      days[index] = updated;
      return { ...prev, days };
    });
  };

  const handleDayRemove = (index: number) => {
    setDraft((prev) => ({
      ...prev,
      days: prev.days.filter((_, i) => i !== index),
    }));
  };

  const handleAddDay = (name: string) => {
    setDraft((prev) => ({
      ...prev,
      days: [
        ...prev.days,
        { id: crypto.randomUUID(), name, exercises: [] },
      ],
    }));
  };

  return (
    <>
      <Card>
        <CardHeader className="pb-2">
          <div className="flex items-start justify-between gap-4">
            <div className="min-w-0 flex-1 space-y-1">
              {isEditing ? (
                <Input
                  value={draft.name}
                  onChange={(e) =>
                    setDraft((prev) => ({ ...prev, name: e.target.value }))
                  }
                  className="h-7 text-base font-semibold"
                />
              ) : (
                <>
                  <h3 className="truncate font-semibold leading-none">
                    {program.name}
                  </h3>
                  <Badge variant="secondary" className="text-xs">
                    {program.days.length}{" "}
                    {program.days.length === 1 ? "day" : "days"}
                  </Badge>
                </>
              )}
            </div>
            <div className="flex items-center gap-1 shrink-0">
              {isEditing ? (
                <>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    onClick={handleSave}
                    disabled={isUpdating}
                  >
                    <Save className="size-4" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    onClick={handleCancel}
                    disabled={isUpdating}
                  >
                    <X className="size-4" />
                  </Button>
                </>
              ) : (
                <>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    onClick={handleStartEdit}
                  >
                    <Pencil className="size-4" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon-sm"
                    className="text-muted-foreground hover:text-destructive"
                    onClick={() => onDelete(program.id)}
                    disabled={isDeleting}
                  >
                    <Trash2 className="size-4" />
                  </Button>
                </>
              )}
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-1">
          {(isEditing ? draft : program).days.map((day, i) => (
            <ProgramDayCard
              key={day.id}
              day={day}
              editing={isEditing}
              onUpdate={(updated) => handleDayUpdate(i, updated)}
              onRemove={() => handleDayRemove(i)}
            />
          ))}
          {isEditing && (
            <Button
              variant="outline"
              size="sm"
              className="mt-2 w-full"
              onClick={() => setAddDayOpen(true)}
            >
              <Plus className="size-4" />
              Add Day
            </Button>
          )}
        </CardContent>
      </Card>
      <AddDayDialog
        open={addDayOpen}
        onOpenChange={setAddDayOpen}
        onAdd={handleAddDay}
      />
    </>
  );
}

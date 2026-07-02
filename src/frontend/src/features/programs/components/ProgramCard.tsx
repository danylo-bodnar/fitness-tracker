import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Trash2 } from "lucide-react";
import type { WorkoutProgramDto } from "../types";
import { ProgramDayCard } from "./ProgramDayCard";

interface ProgramCardProps {
  program: WorkoutProgramDto;
  onDelete: (id: string) => void;
  isDeleting?: boolean;
}

export function ProgramCard({
  program,
  onDelete,
  isDeleting,
}: ProgramCardProps) {
  return (
    <Card>
      <CardHeader className="pb-2">
        <div className="flex items-start justify-between gap-4">
          <div className="space-y-1">
            <h3 className="font-semibold leading-none">{program.name}</h3>
            <Badge variant="secondary" className="text-xs">
              {program.days.length} {program.days.length === 1 ? "day" : "days"}
            </Badge>
          </div>
          <Button
            variant="ghost"
            size="icon"
            className="size-8 shrink-0 text-muted-foreground hover:text-destructive"
            onClick={() => onDelete(program.id)}
            disabled={isDeleting}
          >
            <Trash2 className="size-4" />
          </Button>
        </div>
      </CardHeader>
      <CardContent className="space-y-1">
        {program.days.map((day) => (
          <ProgramDayCard key={day.id} day={day} />
        ))}
      </CardContent>
    </Card>
  );
}

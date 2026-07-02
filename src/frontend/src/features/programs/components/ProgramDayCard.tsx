import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";
import { ChevronDown } from "lucide-react";
import { useState } from "react";
import type { ProgramDayDto } from "../types";
import { ProgramExerciseList } from "./ProgramExerciseList";

interface ProgramDayCardProps {
  day: ProgramDayDto;
}

export function ProgramDayCard({ day }: ProgramDayCardProps) {
  const [open, setOpen] = useState(false);

  return (
    <Collapsible open={open} onOpenChange={setOpen}>
      <CollapsibleTrigger className="flex w-full items-center justify-between rounded-md px-3 py-2 text-sm font-medium hover:bg-muted/50 transition-colors">
        <span>{day.name}</span>
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
      <CollapsibleContent className="px-3 pb-2">
        <ProgramExerciseList exercises={day.exercises} />
      </CollapsibleContent>
    </Collapsible>
  );
}

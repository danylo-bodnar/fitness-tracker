import { useState } from "react";
import {
  usePrograms,
  useCreateProgram,
  useUpdateProgram,
  useDeleteProgram,
  type ProgramDayDto,
} from "@/features/programs";
import { Button } from "@/components/ui/button";
import { EmptyPrograms } from "@/features/programs/components/EmptyPrograms";
import { ProgramCard } from "@/features/programs/components/ProgramCard";
import { ProgramListSkeleton } from "@/features/programs/components/ProgramListSkeleton";
import { CreateProgramDialog } from "@/features/programs/components/CreateProgramDialog";
import { Plus } from "lucide-react";

const MAX_PROGRAMS = 4;

function ProgramsPage() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const { data: programs, isLoading } = usePrograms();
  const { mutate: create, isPending: isCreating } = useCreateProgram();
  const { mutate: remove, isPending: isDeleting } = useDeleteProgram();
  const { mutate: update, isPending: isUpdating } = useUpdateProgram();

  const handleUpdate = (
    id: string,
    data: { name: string; programDays: ProgramDayDto[] },
  ) => update({ id, data });

  const programCount = programs?.length ?? 0;
  const atLimit = programCount >= MAX_PROGRAMS;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-semibold">Programs</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Your training programs and workout days.
          </p>
        </div>
        <Button
          onClick={() => setDialogOpen(true)}
          disabled={atLimit}
        >
          <Plus className="size-4" />
          {atLimit ? "Max 4 Programs" : "New Program"}
        </Button>
      </div>

      {isLoading ? (
        <ProgramListSkeleton />
      ) : !programs || programs.length === 0 ? (
        <EmptyPrograms />
      ) : (
        <div className="space-y-4">
          {programs.map((program) => (
            <ProgramCard
              key={program.id}
              program={program}
              onDelete={remove}
              onUpdate={handleUpdate}
              isDeleting={isDeleting}
              isUpdating={isUpdating}
            />
          ))}
        </div>
      )}

      <CreateProgramDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        onConfirm={(name) =>
          create(
            { name, programDays: [] },
            { onSuccess: () => setDialogOpen(false) },
          )
        }
        isPending={isCreating}
      />
    </div>
  );
}

export default ProgramsPage;

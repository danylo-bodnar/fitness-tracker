import { usePrograms, useDeleteProgram } from "@/features/programs";
import { EmptyPrograms } from "@/features/programs/components/EmptyPrograms";
import { ProgramCard } from "@/features/programs/components/ProgramCard";
import { ProgramListSkeleton } from "@/features/programs/components/ProgramListSkeleton";

function ProgramsPage() {
  const { data: programs, isLoading } = usePrograms();
  const { mutate: remove, isPending } = useDeleteProgram();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Programs</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Your training programs and workout days.
        </p>
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
              isDeleting={isPending}
            />
          ))}
        </div>
      )}
    </div>
  );
}

export default ProgramsPage;

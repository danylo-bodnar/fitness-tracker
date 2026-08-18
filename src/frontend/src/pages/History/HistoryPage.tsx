import { useState } from "react";
import { useWorkoutHistory } from "@/features/workouts";
import type { WorkoutSessionDto } from "@/features/workouts";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";

const PAGE_SIZE = 10;

function HistoryPage() {
  const [page, setPage] = useState(1);
  const { data, isLoading, isFetching } = useWorkoutHistory(page, PAGE_SIZE);

  const totalPages = data ? Math.max(1, Math.ceil(data.totalCount / PAGE_SIZE)) : 1;
  const sessions = data?.items ?? [];

  if (isLoading) return <HistorySkeleton />;

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Workout History</h1>
        <p className="mt-1 text-muted-foreground">All your logged workouts.</p>
      </div>

      {sessions.length === 0 ? (
        <Card>
          <CardContent className="py-12 text-center">
            <p className="text-muted-foreground">
              No workouts logged yet. Log one via the Telegram bot to get started.
            </p>
          </CardContent>
        </Card>
      ) : (
        <>
          <div className="space-y-4">
            {sessions.map((session) => (
              <SessionCard key={session.id} session={session} />
            ))}
          </div>

          <PaginationControls
            page={page}
            totalPages={totalPages}
            disabled={isFetching}
            onPrev={() => setPage((p) => Math.max(1, p - 1))}
            onNext={() => setPage((p) => Math.min(totalPages, p + 1))}
          />
        </>
      )}
    </div>
  );
}

function PaginationControls({
  page,
  totalPages,
  disabled,
  onPrev,
  onNext,
}: {
  page: number;
  totalPages: number;
  disabled: boolean;
  onPrev: () => void;
  onNext: () => void;
}) {
  return (
    <div className="flex items-center justify-between">
      <p className="text-sm text-muted-foreground">
        Page {page} of {totalPages}
      </p>
      <div className="flex gap-2">
        <Button
          variant="outline"
          size="sm"
          onClick={onPrev}
          disabled={disabled || page <= 1}
        >
          Previous
        </Button>
        <Button
          variant="outline"
          size="sm"
          onClick={onNext}
          disabled={disabled || page >= totalPages}
        >
          Next
        </Button>
      </div>
    </div>
  );
}

function SessionCard({ session }: { session: WorkoutSessionDto }) {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="text-lg">
          {new Date(`${session.date}T00:00:00`).toLocaleDateString(undefined, {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-3">
        {session.exercises.map((exercise) => (
          <div key={exercise.exerciseId}>
            <div className="flex items-center gap-2">
              <span className="font-medium capitalize">{exercise.exerciseName}</span>
              <Badge variant="secondary">{exercise.sets.length} sets</Badge>
            </div>
            <ul className="mt-1 list-inside list-disc text-sm text-muted-foreground">
              {exercise.sets.map((set, index) => (
                <li key={index}>
                  Set {index + 1}: {set.weightKg} kg × {set.reps}
                </li>
              ))}
            </ul>
          </div>
        ))}
      </CardContent>
    </Card>
  );
}

function HistorySkeleton() {
  return (
    <div className="space-y-6">
      <div>
        <Skeleton className="h-8 w-48" />
        <Skeleton className="mt-2 h-4 w-72" />
      </div>
      {[...Array(3)].map((_, i) => (
        <Card key={i}>
          <CardHeader>
            <Skeleton className="h-5 w-40" />
          </CardHeader>
          <CardContent className="space-y-2">
            <Skeleton className="h-4 w-32" />
            <Skeleton className="h-4 w-24" />
          </CardContent>
        </Card>
      ))}
    </div>
  );
}

export default HistoryPage;

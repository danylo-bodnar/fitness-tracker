import { useState } from "react";
import {
  usePersonalRecords,
  useExerciseProgress,
  useWeeklyVolume,
} from "@/features/stats";
import { Skeleton } from "@/components/ui/skeleton";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

function ProgressPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Analytics</h1>
        <p className="mt-1 text-muted-foreground">
          Track your progress over time.
        </p>
      </div>
      <PersonalRecordsSection />
      <ExerciseProgressSection />
      <WeeklyVolumeSection />
    </div>
  );
}

function PersonalRecordsSection() {
  const { data: records, isLoading } = usePersonalRecords();

  return (
    <Card>
      <CardHeader>
        <CardTitle>Personal Records</CardTitle>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <div className="space-y-3">
            {[...Array(3)].map((_, i) => (
              <Skeleton key={i} className="h-10 w-full" />
            ))}
          </div>
        ) : !records || records.length === 0 ? (
          <p className="text-muted-foreground">No personal records yet.</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Exercise</TableHead>
                <TableHead>Weight</TableHead>
                <TableHead>Reps</TableHead>
                <TableHead>Est. 1RM</TableHead>
                <TableHead>Date</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {records.map((r) => (
                <TableRow key={r.id}>
                  <TableCell className="font-medium">
                    {r.exerciseName}
                  </TableCell>
                  <TableCell>{r.weightKg} kg</TableCell>
                  <TableCell>{r.reps}</TableCell>
                  <TableCell>{r.estimated1Rm} kg</TableCell>
                  <TableCell>
                    {new Date(r.achievedAt).toLocaleDateString()}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
}

function ExerciseProgressSection() {
  const { data: records } = usePersonalRecords();
  const [selectedExerciseId, setSelectedExerciseId] = useState<string>("");
  const { data: progress, isLoading } = useExerciseProgress(selectedExerciseId);

  const exerciseOptions = records
    ? Array.from(
        new Map(
          records.map((r) => [
            r.exerciseId,
            { id: r.exerciseId, name: r.exerciseName },
          ]),
        ).values(),
      )
    : [];

  return (
    <Card>
      <CardHeader>
        <CardTitle>Exercise Progress</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <Select
          value={selectedExerciseId}
          onValueChange={setSelectedExerciseId}
        >
          <SelectTrigger className="w-full sm:w-64">
            <SelectValue placeholder="Select an exercise" />
          </SelectTrigger>
          <SelectContent>
            {exerciseOptions.map((ex) => (
              <SelectItem key={ex.id} value={ex.id}>
                {ex.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>

        {!selectedExerciseId ? (
          <p className="text-muted-foreground">
            Select an exercise to view progress.
          </p>
        ) : isLoading ? (
          <div className="space-y-3">
            {[...Array(3)].map((_, i) => (
              <Skeleton key={i} className="h-10 w-full" />
            ))}
          </div>
        ) : !progress || progress.length === 0 ? (
          <p className="text-muted-foreground">
            No progress data for this exercise.
          </p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Date</TableHead>
                <TableHead>Max Weight</TableHead>
                <TableHead>Total Volume</TableHead>
                <TableHead>Sets</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {progress.map((p) => (
                <TableRow key={p.id}>
                  <TableCell>
                    {new Date(p.workoutDate).toLocaleDateString()}
                  </TableCell>
                  <TableCell>{p.maxWeightKg} kg</TableCell>
                  <TableCell>{p.totalVolume} kg</TableCell>
                  <TableCell>{p.setCount}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
}

function WeeklyVolumeSection() {
  const { data: volume, isLoading } = useWeeklyVolume(12);

  return (
    <Card>
      <CardHeader>
        <CardTitle>Weekly Volume</CardTitle>
      </CardHeader>
      <CardContent>
        {isLoading ? (
          <div className="space-y-3">
            {[...Array(5)].map((_, i) => (
              <Skeleton key={i} className="h-10 w-full" />
            ))}
          </div>
        ) : !volume || volume.length === 0 ? (
          <p className="text-muted-foreground">No volume data yet.</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Week</TableHead>
                <TableHead>Volume</TableHead>
                <TableHead>Sessions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {volume.map((w) => (
                <TableRow key={w.id}>
                  <TableCell>
                    {new Date(w.weekStart).toLocaleDateString()}
                  </TableCell>
                  <TableCell>{w.totalVolume} kg</TableCell>
                  <TableCell>{w.sessionCount}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </CardContent>
    </Card>
  );
}

export default ProgressPage;

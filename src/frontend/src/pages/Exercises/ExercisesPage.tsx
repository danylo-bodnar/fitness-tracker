import { useState } from "react";
import { useExercises } from "@/features/exercises";
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
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";

function ExercisesPage() {
  const [muscleGroup, setMuscleGroup] = useState("");
  const { data: exercises, isLoading } = useExercises(
    muscleGroup || undefined,
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold">Exercises</h1>
          <p className="mt-1 text-muted-foreground">
            Browse and manage your exercise library.
          </p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>All Exercises</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <Input
            placeholder="Filter by muscle group..."
            value={muscleGroup}
            onChange={(e) => setMuscleGroup(e.target.value)}
            className="w-full sm:w-64"
          />

          {isLoading ? (
            <div className="space-y-3">
              {[...Array(5)].map((_, i) => (
                <Skeleton key={i} className="h-10 w-full" />
              ))}
            </div>
          ) : !exercises || exercises.length === 0 ? (
            <p className="text-muted-foreground">
              {muscleGroup
                ? `No exercises found for "${muscleGroup}".`
                : "No exercises available."}
            </p>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Name</TableHead>
                  <TableHead>Muscle Group</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {exercises.map((ex) => (
                  <TableRow key={ex.id}>
                    <TableCell className="font-medium">{ex.name}</TableCell>
                    <TableCell>
                      {ex.muscleGroup ? (
                        <Badge variant="secondary">{ex.muscleGroup}</Badge>
                      ) : (
                        <span className="text-muted-foreground">—</span>
                      )}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

export default ExercisesPage;

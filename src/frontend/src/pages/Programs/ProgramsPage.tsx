import { usePrograms, useDeleteProgram } from "@/features/programs";
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
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";

function ProgramsPage() {
  const { data: programs, isLoading } = usePrograms();
  const { mutate: remove } = useDeleteProgram();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Programs</h1>
        <p className="mt-1 text-muted-foreground">
          Manage your training programs.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Your Programs</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="space-y-3">
              {[...Array(3)].map((_, i) => (
                <Skeleton key={i} className="h-10 w-full" />
              ))}
            </div>
          ) : !programs || programs.length === 0 ? (
            <p className="text-muted-foreground">
              No programs yet. Create your first program to get started.
            </p>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Name</TableHead>
                  <TableHead>Days</TableHead>
                  <TableHead className="w-24" />
                </TableRow>
              </TableHeader>
              <TableBody>
                {programs.map((program) => (
                  <TableRow key={program.id}>
                    <TableCell className="font-medium">
                      {program.name}
                    </TableCell>
                    <TableCell>
                      <Badge variant="secondary">
                        {program.days.length}{" "}
                        {program.days.length === 1 ? "day" : "days"}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="destructive"
                        size="xs"
                        onClick={() => remove(program.id)}
                      >
                        Delete
                      </Button>
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

export default ProgramsPage;

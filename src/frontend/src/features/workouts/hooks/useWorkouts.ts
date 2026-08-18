import { useQuery } from "@tanstack/react-query";
import { getWorkoutHistory } from "../api/workoutsApi";

export function useWorkoutHistory(page = 1, pageSize = 10) {
  return useQuery({
    queryKey: ["workouts", "history", page, pageSize],
    queryFn: () => getWorkoutHistory(page, pageSize),
  });
}

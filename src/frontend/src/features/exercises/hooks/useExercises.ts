import { useQuery } from "@tanstack/react-query";
import { getExercises, getExercise } from "../api/exercisesApi";

export function useExercises(muscleGroup?: string) {
  return useQuery({
    queryKey: ["exercises", muscleGroup],
    queryFn: () => getExercises(muscleGroup),
  });
}

export function useExercise(id: string) {
  return useQuery({
    queryKey: ["exercises", id],
    queryFn: () => getExercise(id),
    enabled: !!id,
  });
}

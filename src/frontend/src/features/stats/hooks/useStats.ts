import { useQuery } from "@tanstack/react-query";
import {
  getDashboardStats,
  getPersonalRecords,
  getExerciseProgress,
  getWeeklyVolume,
} from "../api/statsApi";

export function useDashboardStats() {
  return useQuery({
    queryKey: ["stats", "dashboard"],
    queryFn: getDashboardStats,
  });
}

export function usePersonalRecords(exerciseId?: string) {
  return useQuery({
    queryKey: ["stats", "personal-records", exerciseId],
    queryFn: () => getPersonalRecords(exerciseId),
  });
}

export function useExerciseProgress(exerciseId: string) {
  return useQuery({
    queryKey: ["stats", "exercise-progress", exerciseId],
    queryFn: () => getExerciseProgress(exerciseId),
    enabled: !!exerciseId,
  });
}

export function useWeeklyVolume(weeks = 12) {
  return useQuery({
    queryKey: ["stats", "weekly-volume", weeks],
    queryFn: () => getWeeklyVolume(weeks),
  });
}

import { apiClient } from "@/lib/apiClient";
import type {
  DashboardStatsDto,
  PersonalRecordDto,
  ExerciseProgressDto,
  WeeklyVolumeDto,
} from "../types";

export async function getDashboardStats(): Promise<DashboardStatsDto> {
  const res = await apiClient.get("/stats/dashboard");
  console.log("dashboard stats raw response:", res.data);
  return res.data;
}

export async function getPersonalRecords(
  exerciseId?: string,
): Promise<PersonalRecordDto[]> {
  const params = exerciseId ? { exerciseId } : {};
  const res = await apiClient.get("/stats/personal-records", { params });
  console.log("personal records raw response:", res.data);

  return res.data;
}

export async function getExerciseProgress(
  exerciseId: string,
): Promise<ExerciseProgressDto[]> {
  const res = await apiClient.get("/stats/exercise-progress", {
    params: { exerciseId },
  });
  console.log("exercise progress raw response:", res.data);
  return res.data;
}

export async function getWeeklyVolume(weeks = 12): Promise<WeeklyVolumeDto[]> {
  const res = await apiClient.get("/stats/weekly-volume", {
    params: { weeks },
  });
  console.log("weekly volume raw response:", res.data);
  return res.data;
}

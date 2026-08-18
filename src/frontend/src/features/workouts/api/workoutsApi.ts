import { apiClient } from "@/lib/apiClient";
import type { PaginatedResponse } from "@/types";
import type { WorkoutSessionDto } from "../types";

export async function getWorkoutHistory(
  page = 1,
  pageSize = 10,
): Promise<PaginatedResponse<WorkoutSessionDto>> {
  const res = await apiClient.get("/workouts", { params: { page, pageSize } });
  return res.data;
}

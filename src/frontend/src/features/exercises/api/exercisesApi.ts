import { apiClient } from "@/lib/apiClient";
import type { Exercise } from "../types";

export async function getExercises(muscleGroup?: string): Promise<Exercise[]> {
  const params = muscleGroup ? { muscleGroup } : {};
  const res = await apiClient.get("/exercises", { params });
  return res.data;
}

export async function getExercise(id: string): Promise<Exercise> {
  const res = await apiClient.get(`/exercises/${id}`);
  return res.data;
}

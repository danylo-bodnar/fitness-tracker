import { apiClient } from "@/lib/apiClient";
import type { Workout } from "../types";

export async function getWorkouts(): Promise<Workout[]> {
  const res = await apiClient.get("/workouts");
  return res.data;
}

export async function logWorkout(data: Omit<Workout, "id">): Promise<Workout> {
  const res = await apiClient.post("/workouts", data);
  return res.data;
}

import { apiClient } from "@/lib/apiClient";
import type { WorkoutProgramDto, CreateProgramRequest, UpdateProgramRequest } from "../types";

export async function getPrograms(): Promise<WorkoutProgramDto[]> {
  const res = await apiClient.get("/programs");
  return res.data;
}

export async function createProgram(
  data: CreateProgramRequest,
): Promise<string> {
  const res = await apiClient.post("/programs", data);
  return res.data;
}

export async function updateProgram(
  id: string,
  data: UpdateProgramRequest,
): Promise<void> {
  await apiClient.put(`/programs/${id}`, data);
}

export async function deleteProgram(id: string): Promise<void> {
  await apiClient.delete(`/programs/${id}`);
}

import { apiClient } from "@/lib/apiClient";
import type { Program } from "../types";

export async function getPrograms(): Promise<Program[]> {
  const res = await apiClient.get("/programs");
  return res.data;
}

export async function createProgram(
  data: Omit<Program, "id">,
): Promise<Program> {
  const res = await apiClient.post("/programs", data);
  return res.data;
}

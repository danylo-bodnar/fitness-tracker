import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  getPrograms,
  createProgram,
  updateProgram,
  deleteProgram,
} from "../api/programsApi";
import type { CreateProgramRequest, UpdateProgramRequest } from "../types";

export function usePrograms() {
  return useQuery({
    queryKey: ["programs"],
    queryFn: getPrograms,
  });
}

export function useCreateProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CreateProgramRequest) => createProgram(data),
    onSuccess: () => {
      toast.success("Program created");
      queryClient.invalidateQueries({ queryKey: ["programs"] });
    },
  });
}

export function useUpdateProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateProgramRequest }) =>
      updateProgram(id, data),
    onSuccess: () => {
      toast.success("Program updated");
      queryClient.invalidateQueries({ queryKey: ["programs"] });
    },
  });
}

export function useDeleteProgram() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteProgram(id),
    onSuccess: () => {
      toast.success("Program deleted");
      queryClient.invalidateQueries({ queryKey: ["programs"] });
    },
  });
}

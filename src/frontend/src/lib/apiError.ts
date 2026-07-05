import { isAxiosError } from "axios";

interface ProblemDetails {
  status: number;
  title: string;
  detail?: string;
  type?: string;
  errors?: Record<string, string[]>;
}

function parseProblemDetails(error: unknown): ProblemDetails | null {
  if (!isAxiosError(error)) return null;
  const data = error.response?.data;
  if (!data?.status) return null;
  return data as ProblemDetails;
}

export function getErrorMessage(error: unknown): string {
  const problem = parseProblemDetails(error);
  if (!problem) return "Something went wrong. Please try again.";

  const fieldErrors = problem.errors;
  if (fieldErrors) {
    const messages = Object.values(fieldErrors).flat().filter(Boolean);
    if (messages.length > 0) return messages.join(". ");
  }

  return problem.detail ?? problem.title ?? "Something went wrong.";
}

export function getFieldErrors(error: unknown): Record<string, string[]> | null {
  const problem = parseProblemDetails(error);
  if (!problem) return null;
  return problem.errors ?? null;
}

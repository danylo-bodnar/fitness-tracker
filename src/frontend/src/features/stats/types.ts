export interface DashboardStatsDto {
  totalSessions: number;
  totalVolumeKg: number;
  lastWorkoutAt: string | null;
}

export interface ExerciseProgressDto {
  id: string;
  exerciseId: string;
  exerciseName: string;
  workoutDate: string;
  maxWeightKg: number;
  totalVolume: number;
  setCount: number;
}

export interface PersonalRecordDto {
  id: string;
  exerciseId: string;
  exerciseName: string;
  weightKg: number;
  reps: number;
  estimated1Rm: number;
  achievedAt: string;
}

export interface WeeklyVolumeDto {
  id: string;
  weekStart: string;
  totalVolume: number;
  sessionCount: number;
}

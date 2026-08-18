export interface WorkoutSessionDto {
  id: string;
  date: string;
  exercises: WorkoutExerciseDto[];
}

export interface WorkoutExerciseDto {
  exerciseId: string;
  exerciseName: string;
  sets: WorkoutSetDto[];
}

export interface WorkoutSetDto {
  weightKg: number;
  reps: number;
}

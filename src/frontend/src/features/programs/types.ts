export interface WorkoutProgramDto {
  id: string;
  name: string;
  days: ProgramDayDto[];
}

export interface ProgramDayDto {
  id: string;
  name: string;
  order: number;
  exercises: ProgramExerciseDto[];
}

export interface ProgramExerciseDto {
  exerciseId: string;
  exerciseName: string;
  targetSets: number;
  targetReps: number;
  order: number;
}

export interface CreateProgramRequest {
  name: string;
  programDays: ProgramDayDto[];
}

export interface UpdateProgramRequest {
  name: string;
  programDays: ProgramDayDto[];
}

export interface Workout {
  id: string;
  name: string;
  date: string;
  exercises: WorkoutExercise[];
}

export interface WorkoutExercise {
  name: string;
  sets: Set[];
}

export interface Set {
  reps: number;
  weight: number;
}

export type {
  DashboardStatsDto,
  PersonalRecordDto,
  ExerciseProgressDto,
  WeeklyVolumeDto,
} from "./types";

export {
  getDashboardStats,
  getPersonalRecords,
  getExerciseProgress,
  getWeeklyVolume,
} from "./api/statsApi";

export {
  useDashboardStats,
  usePersonalRecords,
  useExerciseProgress,
  useWeeklyVolume,
} from "./hooks/useStats";

using MediatR;

namespace FitnessTracker.Application.WorkoutSessions.Commands;

public record LogWorkoutSessionCommand(
    Guid UserId,
    DateOnly Date,
    IReadOnlyList<ExerciseEntry> Exercises
) : IRequest<Guid>;

public record ExerciseEntry(
    Guid ExerciseId,
    string ExerciseName,
    IReadOnlyList<SetEntry> Sets,
    int? SupersetGroupId = null
);

public record SetEntry(decimal WeightKg, int Reps);

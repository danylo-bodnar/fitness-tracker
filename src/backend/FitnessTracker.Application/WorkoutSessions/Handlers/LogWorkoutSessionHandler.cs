using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutSessions.Commands;
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.ValueObjects;
using MediatR;

namespace FitnessTracker.Application.WorkoutSessions.Handlers;

public class LogWorkoutSessionHandler(
    IWorkoutSessionRepository workoutSessionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LogWorkoutSessionCommand, Guid>
{
    public async Task<Guid> Handle(LogWorkoutSessionCommand cmd, CancellationToken ct)
    {
        var session = await workoutSessionRepository.GetByUserAndDateAsync(cmd.UserId, cmd.Date, ct);

        if (session is null)
        {
            session = WorkoutSession.Create(cmd.UserId, cmd.Date);
            workoutSessionRepository.Add(session);
        }

        foreach (var exercise in cmd.Exercises)
        {
            var exerciseLog = session.AddExercise(exercise.ExerciseId, new ExerciseName(exercise.ExerciseName), exercise.SupersetGroupId);

            foreach (var set in exercise.Sets)
                exerciseLog.LogSet(new Weight(set.WeightKg), new Repetitions(set.Reps));

            session.CompleteExercise(exerciseLog);
        }

        await unitOfWork.CommitAsync(ct);

        return session.Id;
    }
}

using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutSessions.Commands;
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;
using MediatR;

namespace FitnessTracker.Application.WorkoutSessions.Handlers;

public class LogWorkoutHandler(
    IWorkoutSessionRepository workoutSessionRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<LogWorkoutCommand, Guid>
{
    public async Task<Guid> Handle(LogWorkoutCommand cmd, CancellationToken cancellationToken)
    {
        var exercise = await exerciseRepository.FindByNameAsync(
            cmd.ExerciseName.Value, cancellationToken)
            ?? throw new ExerciseNotFoundException(cmd.ExerciseName.Value);

        var session = await workoutSessionRepository.GetByUserAndDateAsync(
            cmd.UserId, cmd.Date, cancellationToken);

        if (session is null)
        {
            session = WorkoutSession.Create(cmd.UserId, cmd.Date);
            workoutSessionRepository.Add(session);
        }

        var exerciseLog = session.AddExercise(exercise.Id, exercise.Name, cmd.Date);

        foreach (var reps in cmd.Reps)
        {
            exerciseLog.LogSet(new Weight(cmd.WeightKg), new Repetitions(reps));
        }
        session.CompleteExercise(exerciseLog);

        await unitOfWork.CommitAsync(cancellationToken);

        return session.Id;
    }
}

using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Workouts.Commands;
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Domain.ValueObjects;
using MediatR;

namespace FitnessTracker.Application.Workouts.Handlers;

public class LogWorkoutHandler : IRequestHandler<LogWorkoutCommand, SessionId>
{
    private readonly IWorkoutSessionRepository _workoutSessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LogWorkoutHandler(IWorkoutSessionRepository workoutSessionRepository, IUnitOfWork unitOfWork)
    {
        _workoutSessionRepository = workoutSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SessionId> Handle(LogWorkoutCommand request, CancellationToken cancellationToken)
    {
        var session = await _workoutSessionRepository.GetByUserAndDateAsync(
            request.UserId, request.Date, cancellationToken);

        if (session == null)
        {
            session = WorkoutSession.Create(request.UserId, request.Date);
            _workoutSessionRepository.Add(session);
        }

        var exercise = session.AddExercise(request.ExerciseName, request.Date);
        foreach (var reps in request.Reps)
            exercise.LogSet(new Weight(request.WeightKg), new Repetitions(reps));

        await _unitOfWork.CommitAsync(cancellationToken);

        return session.Id;
    }
}

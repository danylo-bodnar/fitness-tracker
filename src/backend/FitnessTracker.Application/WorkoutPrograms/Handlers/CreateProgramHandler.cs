using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.Exceptions;
using FitnessTracker.Domain.ValueObjects;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Handlers;

public class CreateProgramHandler(IWorkoutProgramRepository workoutProgramRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateProgramCommand, Guid>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository = workoutProgramRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateProgramCommand cmd, CancellationToken cancellationToken)
    {
        var count = await _workoutProgramRepository.CountByUserAsync(cmd.UserId, cancellationToken);

        if (count >= 4)
            throw new WorkoutProgramLimitReachedException(cmd.UserId);

        var program = new WorkoutProgram(
            cmd.UserId,
            cmd.Name,
            [.. cmd.ProgramDays.Select(d => new ProgramDay(d.Name, [.. d.Exercises.Select(e =>
            new ProgramExercise(e.ExerciseId,
            new ExerciseName(e.ExerciseName), e.TargetSets, e.TargetReps, e.Order))]))]
        );

        _workoutProgramRepository.Add(program);

        await _unitOfWork.CommitAsync(cancellationToken);

        return program.Id;
    }
}

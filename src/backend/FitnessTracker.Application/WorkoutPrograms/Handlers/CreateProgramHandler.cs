using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Handlers;

public class CreateProgramHandler : IRequestHandler<CreateProgramCommand, Guid>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProgramHandler(IWorkoutProgramRepository workoutProgramRepository, IUnitOfWork unitOfWork)
    {
        _workoutProgramRepository = workoutProgramRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateProgramCommand cmd, CancellationToken cancellationToken)
    {
        var program = new WorkoutProgram(
            cmd.UserId,
            cmd.Name,
            cmd.ProgramDays.Select(d => new ProgramDay(d.Name, d.Exercises.Select(e =>
            new ProgramExercise(e.ExerciseId,
            new ExerciseName(e.ExerciseName), new Sets(e.TargetSets), new Repetitions(e.TargetReps), e.Order))
            .ToList())).ToList()
        );

        _workoutProgramRepository.Add(program);

        await _unitOfWork.CommitAsync(cancellationToken);

        return program.Id;
    }
}
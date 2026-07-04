using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Application.WorkoutPrograms.Mappers;
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Exceptions;
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
            ProgramDayMapper.ToDomain(cmd.ProgramDays)
        );

        _workoutProgramRepository.Add(program);

        await _unitOfWork.CommitAsync(cancellationToken);

        return program.Id;
    }
}

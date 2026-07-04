using FitnessTracker.Application.Common.Exceptions;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Application.WorkoutPrograms.Mappers;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Handlers;

public class UpdateProgramHandler(IWorkoutProgramRepository workoutProgramRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateProgramCommand>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository = workoutProgramRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _workoutProgramRepository.GetByIdAsync(request.ProgramId, cancellationToken)
            ?? throw new NotFoundException("Workout program not found.");

        if (program.UserId != request.UserId)
        {
            throw new ForbiddenException("You do not have access to this workout program.");
        }

        var newDays = ProgramDayMapper.ToDomain(request.ProgramDays);

        program.Rename(request.Name);
        program.ReplaceDays(newDays);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}

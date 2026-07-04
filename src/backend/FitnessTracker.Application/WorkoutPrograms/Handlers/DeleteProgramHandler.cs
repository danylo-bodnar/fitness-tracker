using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Application.Common.Interfaces;
using MediatR;
using FitnessTracker.Application.Common.Exceptions;

namespace FitnessTracker.Application.WorkoutPrograms.Handlers;

public class DeleteProgramHandler(IWorkoutProgramRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteProgramCommand>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _workoutProgramRepository.GetByIdAsync(request.ProgramId, cancellationToken)
            ?? throw new NotFoundException("Workout program not found.");

        if (program.UserId != request.UserId)
        {
            throw new ForbiddenException("You do not have access to this workout program.");
        }

        _workoutProgramRepository.Delete(program);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}

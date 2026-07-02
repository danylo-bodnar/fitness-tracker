using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Application.Common.Interfaces;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Handlers;

public class DeleteProgramHandler(IWorkoutProgramRepository repository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteProgramCommand>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
    {
        var program = await _workoutProgramRepository.GetByIdAsync(request.ProgramId, cancellationToken);
        if (program == null || program.UserId != request.UserId)
        {
            throw new KeyNotFoundException("Workout program not found or does not belong to the user.");
        }

        _workoutProgramRepository.Delete(program);

        await _unitOfWork.CommitAsync(cancellationToken);
    }
}

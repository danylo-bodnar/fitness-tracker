using FitnessTracker.Application.WorkoutPrograms.Commands;
using FitnessTracker.Application.Common.Interfaces;
using MediatR;

public class DeleteProgramHandler : IRequestHandler<DeleteProgramCommand>
{
    private readonly IWorkoutProgramRepository _workoutProgramRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProgramHandler(IWorkoutProgramRepository repository, IUnitOfWork unitOfWork)
    {
        _workoutProgramRepository = repository;
        _unitOfWork = unitOfWork;
    }

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
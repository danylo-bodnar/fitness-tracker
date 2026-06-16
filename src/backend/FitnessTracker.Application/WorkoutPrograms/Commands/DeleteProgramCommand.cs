using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Commands;

public record DeleteProgramCommand(Guid UserId, Guid ProgramId) : IRequest;
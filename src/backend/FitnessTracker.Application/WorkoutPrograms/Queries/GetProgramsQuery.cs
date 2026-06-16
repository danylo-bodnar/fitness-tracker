using FitnessTracker.Contracts.Dtos;
using MediatR;

public record GetProgramsQuery(Guid UserId)
    : IRequest<List<WorkoutProgramDto>>;
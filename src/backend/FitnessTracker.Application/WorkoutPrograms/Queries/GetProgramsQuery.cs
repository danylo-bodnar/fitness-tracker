using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Queries;

public record GetProgramsQuery(Guid UserId)
    : IRequest<List<WorkoutProgramDto>>;
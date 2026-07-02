using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.WorkoutPrograms.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Handlers;

public class GetProgramsHandler(IWorkoutProgramReadRepository repo)
    : IRequestHandler<GetProgramsQuery, List<WorkoutProgramDto>>
{
    public Task<List<WorkoutProgramDto>> Handle(GetProgramsQuery request, CancellationToken ct)
        => repo.GetByUserAsync(request.UserId, ct);
}

using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Queries;

public class GetProgramsHandler(IWorkoutProgramReadRepository repo)
    : IRequestHandler<GetProgramsQuery, List<WorkoutProgramDto>>
{
    public Task<List<WorkoutProgramDto>> Handle(GetProgramsQuery request, CancellationToken ct)
        => repo.GetByUserAsync(request.UserId);
}

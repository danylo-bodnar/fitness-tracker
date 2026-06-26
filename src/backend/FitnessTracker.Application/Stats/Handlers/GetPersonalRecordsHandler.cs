using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Stats.Queries;
using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Stats.Handlers;

public sealed class GetPersonalRecordsHandler(IStatsRepository repo)
    : IRequestHandler<GetPersonalRecordsQuery, List<PersonalRecordDto>>
{
    public Task<List<PersonalRecordDto>> Handle(GetPersonalRecordsQuery request, CancellationToken ct)
        => repo.GetPersonalRecordsAsync(request.UserId, request.ExerciseId, ct);
}

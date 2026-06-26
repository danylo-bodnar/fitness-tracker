using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.Stats.Queries;

public record GetDashboardQuery(Guid UserId) : IRequest<DashboardStatsDto>;

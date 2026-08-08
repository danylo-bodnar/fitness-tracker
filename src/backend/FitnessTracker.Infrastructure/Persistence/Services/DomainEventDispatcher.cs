using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Events;
using FitnessTracker.Domain;
using FitnessTracker.Domain.Events;
using FitnessTracker.Domain.Services;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Services;

public class DomainEventDispatcher(
    AppDbContext db,
    IPublishEndpoint publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(CancellationToken cancellationToken)
    {
        var aggregates = db.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.PopEvents())
            {
                switch (domainEvent)
                {
                    case ExercisePerformed e:
                        await PublishExerciseLoggedEvent(e, cancellationToken);
                        break;
                }
            }
        }
    }

    private async Task PublishExerciseLoggedEvent(ExercisePerformed e, CancellationToken ct)
    {
        var bestSet = e.Sets
            .OrderByDescending(s => s.WeightKg)
            .ThenBy(s => s.Reps)
            .First();

        var estimated1Rm = OneRepMaxEstimator.Epley(bestSet.WeightKg, bestSet.Reps);

        await publisher.Publish(new ExerciseLoggedEvent(
            EventId: e.EventId,
            UserId: e.UserId,
            ExerciseId: e.ExerciseId,
            ExerciseName: e.ExerciseName.Value,
            Date: e.Date,
            MaxWeightKg: bestSet.WeightKg,
            Estimated1Rm: estimated1Rm,
            BestSetReps: bestSet.Reps,
            TotalVolume: e.Sets.Sum(s => s.WeightKg * s.Reps),
            SetCount: e.Sets.Count,
            SupersetGroupId: e.SupersetGroupId
        ), ct);
    }
}

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
                    case WorkoutSessionCompleted e:
                        await PublishWorkoutSessionCompletedEvent(e, cancellationToken);
                        break;
                }
            }
        }
    }

    private async Task PublishExerciseLoggedEvent(ExercisePerformed e, CancellationToken ct)
    {
        await publisher.Publish(new ExerciseLoggedEvent(
            EventId: e.EventId,
            UserId: e.UserId,
            ExerciseId: e.ExerciseId,
            ExerciseName: e.ExerciseName.Value,
            Date: e.Date,
            MaxWeightKg: e.MaxWeightKg,
            Estimated1Rm: e.Estimated1Rm,
            BestSetReps: e.BestSetReps,
            TotalVolume: e.TotalVolume,
            SetCount: e.SetCount,
            SupersetGroupId: e.SupersetGroupId
        ), ct);
    }

    private async Task PublishWorkoutSessionCompletedEvent(WorkoutSessionCompleted e, CancellationToken ct)
    {
        await publisher.Publish(new WorkoutSessionCompletedEvent(
            EventId: e.EventId,
            SessionId: e.SessionId,
            ExerciseCount: e.ExerciseCount,
            UserId: e.UserId,
            Date: e.Date,
            TotalVolume: e.TotalVolume
        ), ct);
    }
}

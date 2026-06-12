using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.ToTable("workout_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new SessionId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Date)
            .HasColumnType("date");

        builder.Property(x => x.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Metadata.FindNavigation(nameof(WorkoutSession.Exercises))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

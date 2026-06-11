using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class ExerciseLogConfiguration : IEntityTypeConfiguration<ExerciseLog>
{
    public void Configure(EntityTypeBuilder<ExerciseLog> builder)
    {
        builder.ToTable("exercise_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasConversion(
                name => name.Value,
                value => new(value))
            .HasColumnName("exercise_name")
            .HasMaxLength(100);

        builder.HasOne<WorkoutSession>()
            .WithMany(x => x.Exercises)
            .HasForeignKey("WorkoutSessionId")
            .IsRequired();

        builder.Metadata.FindNavigation(nameof(ExerciseLog.Sets))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

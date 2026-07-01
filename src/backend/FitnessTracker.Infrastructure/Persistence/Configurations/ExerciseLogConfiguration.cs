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
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.ExerciseId)
            .HasColumnName("exercise_id");

        builder.Property(x => x.ExerciseName)
            .HasConversion(
                name => name.Value,
                value => new(value))
            .HasColumnName("exercise_name")
            .HasMaxLength(100);

        builder.Property("WorkoutSessionId")
            .HasColumnName("workout_session_id");

        builder.HasOne<WorkoutSession>()
            .WithMany(x => x.Exercises)
            .HasForeignKey("WorkoutSessionId")
            .IsRequired();

        builder.HasOne<Exercise>()
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .IsRequired();

        builder.HasMany<Set>("_sets")
            .WithOne()
            .HasForeignKey("ExerciseLogId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}

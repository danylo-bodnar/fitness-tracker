using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class ProgramExerciseConfiguration : IEntityTypeConfiguration<ProgramExercise>
{
    public void Configure(EntityTypeBuilder<ProgramExercise> builder)
    {
        builder.ToTable("program_exercises");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.ExerciseId)
            .IsRequired()
            .HasColumnName("exercise_id");

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName("order");

        builder.Property(x => x.ExerciseName)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("exercise_name")
            .HasConversion(
                v => v.Value,
                v => new ExerciseName(v));

        builder.Property(x => x.TargetSets)
            .IsRequired()
            .HasColumnName("target_sets");

        builder.Property(x => x.TargetReps)
            .IsRequired()
            .HasColumnName("target_reps");

        builder.Property("ProgramDayId")
            .HasColumnName("program_day_id");
    }
}
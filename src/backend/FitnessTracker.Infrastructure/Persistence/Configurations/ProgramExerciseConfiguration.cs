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
            .ValueGeneratedNever();

        builder.Property(x => x.ExerciseId)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.ExerciseName)
            .IsRequired()
            .HasMaxLength(100)
            .HasConversion(
                v => v.Value,
                v => new ExerciseName(v));

        builder.Property(x => x.TargetSets)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Sets(v));

        builder.Property(x => x.TargetReps)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new Repetitions(v));
    }
}
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercises");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasConversion(
                name => name.Value,
                value => new(value))
            .HasColumnName("exercise_name")
            .HasMaxLength(100);

        builder.Property(x => x.MuscleGroup)
            .HasMaxLength(50);

        builder.HasData(
            new Exercise(new ExerciseName("bicep curl"), "Arms") { Id = Guid.Parse("00000001-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("hammer curl"), "Arms") { Id = Guid.Parse("00000002-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("squat"), "Legs") { Id = Guid.Parse("00000003-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("leg press"), "Legs") { Id = Guid.Parse("00000004-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("leg curl"), "Legs") { Id = Guid.Parse("00000005-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("calf raises"), "Legs") { Id = Guid.Parse("00000006-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("romanian deadlift"), "Legs") { Id = Guid.Parse("00000007-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("bench press"), "Chest") { Id = Guid.Parse("00000008-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("incline dumbbell press"), "Chest") { Id = Guid.Parse("00000009-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("dips"), "Chest") { Id = Guid.Parse("0000000a-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("lateral raises"), "Shoulders") { Id = Guid.Parse("0000000b-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("triceps pushdown"), "Arms") { Id = Guid.Parse("0000000c-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("triceps extension"), "Arms") { Id = Guid.Parse("0000000d-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("pull-ups"), "Back") { Id = Guid.Parse("0000000e-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("barbell row"), "Back") { Id = Guid.Parse("0000000f-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("cable row"), "Back") { Id = Guid.Parse("00000010-0000-0000-0000-000000000001") },
            new Exercise(new ExerciseName("machine row"), "Back") { Id = Guid.Parse("00000011-0000-0000-0000-000000000001") }
        );
    }
}

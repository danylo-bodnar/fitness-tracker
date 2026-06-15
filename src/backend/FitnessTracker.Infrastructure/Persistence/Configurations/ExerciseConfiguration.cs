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
             // ── Biceps ───────────────────────────────────────────
             new Exercise(new ExerciseName("bicep curl"), "Biceps") { Id = Guid.Parse("00000001-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("hammer curl"), "Biceps") { Id = Guid.Parse("00000002-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("incline dumbbell curl"), "Biceps") { Id = Guid.Parse("00000001-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("cable curl"), "Biceps") { Id = Guid.Parse("00000002-0000-0000-0000-000000000002") },

             // ── Triceps ──────────────────────────────────────────
             new Exercise(new ExerciseName("triceps pushdown"), "Triceps") { Id = Guid.Parse("0000000c-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("triceps extension"), "Triceps") { Id = Guid.Parse("0000000d-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("skull crusher"), "Triceps") { Id = Guid.Parse("00000003-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("close grip bench press"), "Triceps") { Id = Guid.Parse("00000004-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("dips"), "Triceps") { Id = Guid.Parse("0000000a-0000-0000-0000-000000000001") },

             // ── Chest ────────────────────────────────────────────
             new Exercise(new ExerciseName("bench press"), "Chest") { Id = Guid.Parse("00000008-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("incline bench press"), "Chest") { Id = Guid.Parse("00000005-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("incline dumbbell press"), "Chest") { Id = Guid.Parse("00000009-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("cable fly"), "Chest") { Id = Guid.Parse("00000006-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("pec deck"), "Chest") { Id = Guid.Parse("00000007-0000-0000-0000-000000000002") },

             // ── Shoulders ────────────────────────────────────────
             new Exercise(new ExerciseName("lateral raises"), "Shoulders") { Id = Guid.Parse("0000000b-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("overhead press"), "Shoulders") { Id = Guid.Parse("00000008-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("front raises"), "Shoulders") { Id = Guid.Parse("00000009-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("face pull"), "Shoulders") { Id = Guid.Parse("0000000a-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("arnold press"), "Shoulders") { Id = Guid.Parse("0000000b-0000-0000-0000-000000000002") },

             // ── Back ─────────────────────────────────────────────
             new Exercise(new ExerciseName("pull-ups"), "Back") { Id = Guid.Parse("0000000e-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("lat pulldown"), "Back") { Id = Guid.Parse("0000000c-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("barbell row"), "Back") { Id = Guid.Parse("0000000f-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("cable row"), "Back") { Id = Guid.Parse("00000010-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("machine row"), "Back") { Id = Guid.Parse("00000011-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("deadlift"), "Back") { Id = Guid.Parse("0000000d-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("t-bar row"), "Back") { Id = Guid.Parse("0000000e-0000-0000-0000-000000000002") },

             // ── Legs ─────────────────────────────────────────────
             new Exercise(new ExerciseName("squat"), "Legs") { Id = Guid.Parse("00000003-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("leg press"), "Legs") { Id = Guid.Parse("00000004-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("leg curl"), "Legs") { Id = Guid.Parse("00000005-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("leg extension"), "Legs") { Id = Guid.Parse("0000000f-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("calf raises"), "Legs") { Id = Guid.Parse("00000006-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("romanian deadlift"), "Legs") { Id = Guid.Parse("00000007-0000-0000-0000-000000000001") },
             new Exercise(new ExerciseName("lunges"), "Legs") { Id = Guid.Parse("00000010-0000-0000-0000-000000000002") },
             new Exercise(new ExerciseName("hip thrust"), "Legs") { Id = Guid.Parse("00000011-0000-0000-0000-000000000002") }
         );
    }
}

using FitnessTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class SetConfiguration : IEntityTypeConfiguration<Set>
{
    public void Configure(EntityTypeBuilder<Set> builder)
    {
        builder.ToTable("sets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.Weight)
            .HasConversion(
                weight => weight.Kg,
                value => new(value))
            .HasColumnType("decimal(5,2)")
            .HasColumnName("weight");

        builder.Property(x => x.Repetitions)
            .HasConversion(
                reps => reps.Value,
                value => new(value))
            .HasColumnName("repetitions");

        builder.Property("ExerciseLogId")
            .HasColumnName("exercise_log_id");

        builder.HasOne<ExerciseLog>()
            .WithMany(x => x.Sets)
            .HasForeignKey("ExerciseLogId")
            .IsRequired();
    }
}

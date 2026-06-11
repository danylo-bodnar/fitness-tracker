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
            .ValueGeneratedNever();

        builder.Property(x => x.Weight)
            .HasConversion(
                weight => weight.Kg,
                value => new(value))
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.Repetitions)
            .HasConversion(
                reps => reps.Value,
                value => new(value));

        builder.HasOne<ExerciseLog>()
            .WithMany(x => x.Sets)
            .HasForeignKey("ExerciseLogId")
            .IsRequired();
    }
}

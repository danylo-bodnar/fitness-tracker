using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class WorkoutProgramConfiguration : IEntityTypeConfiguration<WorkoutProgram>
{
    public void Configure(EntityTypeBuilder<WorkoutProgram> builder)
    {
        builder.ToTable("workout_programs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Ignore(x => x.Days);

        builder.HasMany<ProgramDay>("_days")
            .WithOne()
            .HasForeignKey("WorkoutProgramId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_days")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
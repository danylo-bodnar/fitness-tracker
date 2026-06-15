using FitnessTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class ProgramDayConfiguration : IEntityTypeConfiguration<ProgramDay>
{
    public void Configure(EntityTypeBuilder<ProgramDay> builder)
    {
        builder.ToTable("program_days");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany<ProgramExercise>("_exercises")
            .WithOne()
            .HasForeignKey("ProgramDayId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_exercises")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(x => x.Exercises);
    }
}
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
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(x => x.Order)
            .IsRequired()
            .HasColumnName("order");

        builder.Property("WorkoutProgramId")
            .HasColumnName("workout_program_id");

        builder.HasMany(x => x.Exercises)
            .WithOne()
            .HasForeignKey("ProgramDayId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Exercises)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracker.Infrastructure.Persistence.Configurations;

public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.ToTable("workout_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("id");

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.Property(x => x.Date)
            .HasColumnType("date")
            .HasColumnName("date");

        builder.HasMany(typeof(ExerciseLog), "_exercises")
            .WithOne()
            .HasForeignKey("WorkoutSessionId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_exercises")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
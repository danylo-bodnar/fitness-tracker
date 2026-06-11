using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence;

public class WriteDbContext : DbContext
{
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkoutSessionConfiguration());
        modelBuilder.ApplyConfiguration(new ExerciseLogConfiguration());
        modelBuilder.ApplyConfiguration(new SetConfiguration());
    }
}

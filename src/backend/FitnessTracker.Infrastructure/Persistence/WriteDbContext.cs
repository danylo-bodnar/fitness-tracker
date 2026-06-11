using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence;

public class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkoutSessionConfiguration());
        modelBuilder.ApplyConfiguration(new ExerciseLogConfiguration());
        modelBuilder.ApplyConfiguration(new SetConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}

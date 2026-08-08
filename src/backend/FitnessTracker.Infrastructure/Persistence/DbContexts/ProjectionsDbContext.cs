using FitnessTracker.Infrastructure.Persistence.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.DbContexts;

public class ProjectionsDbContext(DbContextOptions<ProjectionsDbContext> options) : DbContext(options)
{
    public DbSet<PersonalRecordReadModel> UserPRs => Set<PersonalRecordReadModel>();
    public DbSet<ExerciseProgressReadModel> ExerciseProgress => Set<ExerciseProgressReadModel>();
    public DbSet<WeeklyVolumeReadModel> WeeklyVolume => Set<WeeklyVolumeReadModel>();
    public DbSet<DashboardStatsReadModel> DashboardStats => Set<DashboardStatsReadModel>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersonalRecordReadModel>(b =>
        {
            b.ToTable("user_prs");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.ExerciseId }).IsUnique();
            b.Property(x => x.WeightKg).HasPrecision(6, 2);
            b.Property(x => x.Estimated1RM).HasPrecision(6, 2);
            b.Property(x => x.ExerciseName).HasMaxLength(100);
        });

        modelBuilder.Entity<ExerciseProgressReadModel>(b =>
        {
            b.ToTable("exercise_progress");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.ExerciseId });
            b.Property(x => x.MaxWeightKg).HasPrecision(6, 2);
            b.Property(x => x.TotalVolume).HasPrecision(10, 2);
            b.Property(x => x.ExerciseName).HasMaxLength(100);
        });

        modelBuilder.Entity<WeeklyVolumeReadModel>(b =>
        {
            b.ToTable("weekly_volume");
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.WeekStart }).IsUnique();
            b.Property(x => x.TotalVolume).HasPrecision(10, 2);
        });

        modelBuilder.Entity<DashboardStatsReadModel>(b =>
        {
            b.ToTable("dashboard_stats");
            b.HasKey(x => x.UserId);

            b.Property(x => x.TotalVolumeKg)
                .HasPrecision(10, 2);
        });

        modelBuilder.Entity<ProcessedMessage>(b =>
        {
            b.ToTable("processed_messages");
            b.HasKey(x => new { x.ConsumerName, x.EventId });
            b.Property(x => x.ConsumerName).HasMaxLength(100);
            b.Property(x => x.ProcessedAt).IsRequired();
        });
    }
}

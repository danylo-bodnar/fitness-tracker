using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FitnessTracker.Infrastructure.Persistence.DbContexts;

public class WriteDesignTimeDbContextFactory : IDesignTimeDbContextFactory<WriteDbContext>
{
    public WriteDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=fitness_tracker;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<WriteDbContext>()
            .UseNpgsql(connection)
            .Options;

        return new WriteDbContext(options);
    }
}

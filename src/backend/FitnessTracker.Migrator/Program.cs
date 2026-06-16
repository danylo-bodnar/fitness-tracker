using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var cs = context.Configuration.GetConnectionString("fitness-tracker");

        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(cs));

        services.AddDbContext<ProjectionsDbContext>(o =>
            o.UseNpgsql(cs));
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var appDb = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var projectionsDb = scope.ServiceProvider.GetRequiredService<ProjectionsDbContext>();

    await appDb.Database.MigrateAsync();
    await projectionsDb.Database.MigrateAsync();

    Console.WriteLine("✅ Databases migrated successfully");
}

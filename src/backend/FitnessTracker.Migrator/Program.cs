using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var cs = context.Configuration.GetConnectionString("fitness-tracker");

        services.AddDbContext<WriteDbContext>(o =>
            o.UseNpgsql(cs));

        services.AddDbContext<ReadDbContext>(o =>
            o.UseNpgsql(cs));
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var writeDb = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
    var readDb = scope.ServiceProvider.GetRequiredService<ReadDbContext>();

    await writeDb.Database.MigrateAsync();
    await readDb.Database.MigrateAsync();

    Console.WriteLine("✅ Databases migrated successfully");
}

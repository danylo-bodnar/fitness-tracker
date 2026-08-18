using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Migrator;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var cs = context.Configuration.GetConnectionString("fitness-tracker")
            ?? context.Configuration["DATABASE_CONNECTION"]
            ?? throw new InvalidOperationException("No connection string");

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

    var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    if (environment.IsDevelopment() && configuration["SeedData:Enabled"] != "false")
    {
        var seedSection = configuration.GetSection("SeedData");
        var seedChatId = long.TryParse(seedSection["TelegramChatId"], out var chatId)
            ? chatId
            : 123456789L;
        var seedUsername = seedSection["TelegramUsername"] ?? "dev";

        await SeedData.SeedAsync(appDb, projectionsDb, seedChatId, seedUsername, CancellationToken.None);
        Console.WriteLine("✅ Dev seed data inserted");
    }
}

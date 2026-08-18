using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Infrastructure.Auth;
using FitnessTracker.Infrastructure.Messaging.Consumers;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.Repositories;
using FitnessTracker.Infrastructure.Persistence.Services;
using FitnessTracker.Infrastructure.RateLimiting;
using FitnessTracker.Infrastructure.RateLimiting.SlidingWindow;
using FitnessTracker.Infrastructure.RateLimiting.TokenBucket;
using FitnessTracker.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace FitnessTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("fitness-tracker")
            ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
            ?? throw new InvalidOperationException("No database connection string found");

        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));
        services.AddDbContext<ProjectionsDbContext>(o => o.UseNpgsql(connectionString));

        services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
        services.AddScoped<IWorkoutSessionReadRepository, WorkoutSessionReadRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExerciseReadRepository, ExerciseReadRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IWorkoutProgramRepository, WorkoutProgramRepository>();
        services.AddScoped<IWorkoutProgramReadRepository, WorkoutProgramReadRepository>();
        services.AddScoped<IStatsRepository, StatsRepository>();
        services.AddScoped<ILoginSessionRepository, RedisLoginSessionRepository>();
        services.AddScoped<IRefreshSessionRepository, RedisRefreshSessionRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddScoped<IJwtService, JwtService>();

        services.AddSingleton<ILoginSessionNotifier, LoginSessionNotifier>();
        services.AddSingleton<IAuthCodeStore, RedisAuthCodeStore>();

        var redisConnection =
            configuration["REDIS_CONNECTION"]
            ?? configuration.GetConnectionString("redis")
            ?? throw new InvalidOperationException("Redis connection string not found");

        services.AddSingleton<ConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(redisConnection);
            options.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(options);
        });
        services.AddSingleton<IConnectionMultiplexer>(s => s.GetRequiredService<ConnectionMultiplexer>());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<AnalyticsConsumer>();
            x.AddConsumer<PersonalRecordConsumer>();
            x.AddConsumer<NotificationConsumer>();

            x.AddEntityFrameworkOutbox<AppDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((ctx, cfg) =>
             {
                 var rabbitMqConnection =
                    configuration.GetConnectionString("rabbitmq")
                    ?? configuration["RABBITMQ_CONNECTION"]
                    ?? throw new InvalidOperationException("RabbitMQ connection string not found");

                 cfg.Host(rabbitMqConnection);
                 cfg.ConfigureEndpoints(ctx);
             });

        });

        services.Configure<SlidingWindowOptions>(
            configuration.GetSection(SlidingWindowOptions.SectionName));
        services.Configure<TokenBucketOptions>(
            configuration.GetSection(TokenBucketOptions.SectionName));

        services.AddSingleton<ISlidingWindowRateLimiter>(sp =>
        {
            var redis = sp.GetRequiredService<IConnectionMultiplexer>();
            var options = sp.GetRequiredService<IOptions<SlidingWindowOptions>>().Value;
            var scriptPath = Path.Combine(AppContext.BaseDirectory, "RateLimiting", "Lua", "sliding_window_counter.lua");

            return new RedisSlidingWindowCounterRateLimiter(
                new RedisScriptRunner(redis, scriptPath),
                options);
        });

        services.AddSingleton<ITokenBucketRateLimiter>(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                var options = sp.GetRequiredService<IOptions<TokenBucketOptions>>().Value;
                var scriptPath = Path.Combine(AppContext.BaseDirectory, "RateLimiting", "Lua", "token_bucket.lua");

                return new RedisTokenBucketRateLimiter(
                    new RedisScriptRunner(redis, scriptPath),
                    options);
            });

        return services;
    }
}

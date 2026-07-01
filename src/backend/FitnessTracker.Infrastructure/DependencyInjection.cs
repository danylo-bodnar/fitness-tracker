using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Infrastructure.Auth;
using FitnessTracker.Infrastructure.Messaging.Consumers;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using FitnessTracker.Infrastructure.Persistence.Repositories;
using FitnessTracker.Infrastructure.Persistence.Services;
using FitnessTracker.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FitnessTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(o => o.UseNpgsql());
        services.AddDbContext<ProjectionsDbContext>(o => o.UseNpgsql());

        services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
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

        return services;
    }
}

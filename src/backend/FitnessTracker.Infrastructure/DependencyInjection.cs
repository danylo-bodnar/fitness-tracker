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
        var connectionString =
            configuration.GetConnectionString("fitness-tracker")
            ?? configuration["DATABASE_CONNECTION"]
            ?? throw new InvalidOperationException("No connection string");

        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));
        services.AddDbContext<ProjectionsDbContext>(o => o.UseNpgsql(connectionString));

        services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IWorkoutProgramRepository, WorkoutProgramRepository>();
        services.AddScoped<IWorkoutProgramReadRepository, WorkoutProgramReadRepository>();
        services.AddScoped<ILoginSessionRepository, RedisLoginSessionRepository>();
        services.AddScoped<ILoginEventPublisher, RedisLoginEventPublisher>();
        services.AddScoped<ILoginEventSubscriber, RedisLoginEventSubscriber>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.AddScoped<IJwtService, JwtService>();

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

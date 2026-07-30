using FitnessTracker.Application.Common.Behaviors;
using FitnessTracker.Application.WorkoutSessions.Handlers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<LogWorkoutHandler>());

        services.AddValidatorsFromAssemblyContaining<LogWorkoutHandler>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}

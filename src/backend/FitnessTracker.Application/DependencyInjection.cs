using FitnessTracker.Application.WorkoutSessions.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<LogWorkoutHandler>());
        return services;
    }
}

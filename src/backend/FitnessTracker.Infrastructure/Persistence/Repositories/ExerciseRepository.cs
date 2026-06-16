using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Domain.Entities;
using FitnessTracker.Domain.ValueObjects;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class ExerciseRepository(WriteDbContext db) : IExerciseRepository
{
    public async Task<Exercise?> FindByNameAsync(string name, CancellationToken ct = default)
        => await db.Exercises
            .FirstOrDefaultAsync(e => e.Name == new ExerciseName(name), ct);

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Exercises
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task AddAsync(Exercise exercise, CancellationToken ct = default)
        => await db.Exercises.AddAsync(exercise, ct);
}

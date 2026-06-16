using FitnessTracker.Domain.Aggregates;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class WorkoutProgramRepository : IWorkoutProgramRepository
{
    private readonly AppDbContext _db;

    public WorkoutProgramRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<WorkoutProgram?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.WorkoutPrograms
            .Include(p => p.Days)
            .ThenInclude(d => d.Exercises)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public void Add(WorkoutProgram program)
        => _db.WorkoutPrograms.Add(program);

    public void Delete(WorkoutProgram program)
        => _db.WorkoutPrograms.Remove(program);
}
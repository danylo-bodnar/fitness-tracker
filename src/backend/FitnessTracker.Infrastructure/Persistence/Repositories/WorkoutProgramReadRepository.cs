using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Contracts.Dtos;
using FitnessTracker.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure.Persistence.Repositories;

public class WorkoutProgramReadRepository(AppDbContext db)
    : IWorkoutProgramReadRepository
{
    public async Task<List<WorkoutProgramDto>> GetByUserAsync(Guid userId)
    {
        return await db.WorkoutPrograms
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new WorkoutProgramDto
            {
                Id = p.Id,
                Name = p.Name,
                Days = p.Days.Select(d => new ProgramDayDto(
                    d.Id,
                    d.Name,
                    d.Exercises.Select(e => new ProgramExerciseDto(
                        e.Id,
                        e.ExerciseName.Value,
                        e.TargetSets.Value,
                        e.TargetReps.Value,
                        e.Order
                    )).ToList()
                )).ToList()
            })
            .ToListAsync();
    }
}

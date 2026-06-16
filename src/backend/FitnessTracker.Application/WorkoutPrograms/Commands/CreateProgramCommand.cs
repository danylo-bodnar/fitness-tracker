using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Commands;

public record CreateProgramCommand(
    Guid UserId,
    string Name,
    IReadOnlyList<ProgramDayDto> ProgramDays
) : IRequest<Guid>;
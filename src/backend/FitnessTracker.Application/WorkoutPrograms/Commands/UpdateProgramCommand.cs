using FitnessTracker.Contracts.Dtos;
using MediatR;

namespace FitnessTracker.Application.WorkoutPrograms.Commands;

public record UpdateProgramCommand(
    Guid UserId,
    Guid ProgramId,
    string Name,
    IReadOnlyList<ProgramDayDto> ProgramDays
) : IRequest;



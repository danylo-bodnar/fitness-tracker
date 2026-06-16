namespace FitnessTracker.Contracts.Dtos;

public class WorkoutProgramDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public List<ProgramDayDto> Days { get; set; } = [];
}

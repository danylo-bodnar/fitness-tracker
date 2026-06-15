namespace FitnessTracker.Domain.Exceptions;

public class ProgramDayNotFoundException : DomainException
{
    public ProgramDayNotFoundException()
        : base("Program day not found.")
    {
    }

    public ProgramDayNotFoundException(Guid dayId)
        : base($"Program day with id {dayId} not found.")
    {
    }
}

namespace FitnessTracker.Domain.Exceptions;

public class ProgramDayAlreadyExists : DomainException
{
    public ProgramDayAlreadyExists(string name)
        : base($"A program day with the name '{name}' already exists.")
    {
    }
}
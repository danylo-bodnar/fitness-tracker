namespace FitnessTracker.Domain.Exceptions;

public class ProgramDayAlreadyExistsException : DomainException
{
    public ProgramDayAlreadyExistsException(string name)
        : base($"A program day with the name '{name}' already exists.")
    {
    }
}
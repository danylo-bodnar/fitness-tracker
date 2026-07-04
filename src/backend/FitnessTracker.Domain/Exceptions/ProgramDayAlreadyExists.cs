namespace FitnessTracker.Domain.Exceptions;

public class ProgramDayAlreadyExistsException(string name) : DomainException($"A program day with the name '{name}' already exists.")
{
}

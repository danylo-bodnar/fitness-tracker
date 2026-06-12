namespace FitnessTracker.Domain.Exceptions;

public class ExerciseNotFoundException(string name) : DomainException($"Exercise '{name}' not found")
{
}

namespace FitnessTracker.Domain.Exceptions;

public class InvalidExerciseNameException(string message) : DomainException(message)
{
}

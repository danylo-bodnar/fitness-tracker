namespace FitnessTracker.Domain.Exceptions;

public class InvalidExerciseNameException : DomainException
{
    public InvalidExerciseNameException(string message) : base(message)
    {
    }
}

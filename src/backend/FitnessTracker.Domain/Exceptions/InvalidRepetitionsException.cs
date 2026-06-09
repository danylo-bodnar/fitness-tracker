namespace FitnessTracker.Domain.Exceptions;

public class InvalidRepetitionsException(int reps) : DomainException($"Invalid repetitions: {reps}. Repetitions must be positive.")
{
}

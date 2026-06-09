namespace FitnessTracker.Domain.Exceptions;

public class DuplicateExerciseException(string exerciseName) : DomainException($"Exercise '{exerciseName}' is already logged in this session.")
{
}

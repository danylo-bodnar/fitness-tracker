namespace FitnessTracker.Domain.Exceptions;

public class WorkoutProgramLimitReachedException(Guid userId)
    : DomainException($"User '{userId}' has reached the maximum number of workout programs (4).")
{
}

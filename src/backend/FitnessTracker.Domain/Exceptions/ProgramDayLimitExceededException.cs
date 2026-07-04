namespace FitnessTracker.Domain.Exceptions;

public sealed class ProgramDayLimitExceededException(int limit)
        : DomainException($"A workout program cannot contain more than {limit} days.")
{
}

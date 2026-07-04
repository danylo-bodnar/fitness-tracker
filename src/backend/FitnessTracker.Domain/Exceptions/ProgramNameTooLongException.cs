namespace FitnessTracker.Domain.Exceptions;

public sealed class ProgramNameTooLongException(int maxLength)
    : DomainException($"Program name cannot exceed {maxLength} characters.")
{
}

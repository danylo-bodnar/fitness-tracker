namespace FitnessTracker.Domain.Exceptions;

public sealed class ProgramNameEmptyException
    : DomainException
{
    public ProgramNameEmptyException()
        : base("Program name cannot be empty.")
    {
    }
}

namespace FitnessTracker.Domain.Exceptions;

public class PastSessionModificationException : DomainException
{
    public PastSessionModificationException()
        : base("Cannot modify past sessions.")
    {
    }
}

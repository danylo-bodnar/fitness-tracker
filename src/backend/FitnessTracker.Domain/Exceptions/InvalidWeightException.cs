namespace FitnessTracker.Domain.Exceptions;

public class InvalidWeightException : DomainException
{
    public InvalidWeightException(string message) : base(message)
    {
    }

    public InvalidWeightException(decimal kg)
        : base($"Invalid weight: {kg}. Weight must be positive.")
    {
    }
}

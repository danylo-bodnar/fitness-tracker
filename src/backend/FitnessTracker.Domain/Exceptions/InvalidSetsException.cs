namespace FitnessTracker.Domain.Exceptions;

public class InvalidSetsException(int sets) : DomainException($"Invalid sets: {sets}. Sets must be positive.")
{
}

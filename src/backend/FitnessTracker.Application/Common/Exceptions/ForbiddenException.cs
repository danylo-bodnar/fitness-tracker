namespace FitnessTracker.Application.Common.Exceptions;

public class ForbiddenException(string message) : ApplicationException(message)
{
}

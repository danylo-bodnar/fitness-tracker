namespace FitnessTracker.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId);

}
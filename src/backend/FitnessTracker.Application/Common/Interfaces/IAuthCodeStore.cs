namespace FitnessTracker.Application.Common.Interfaces;

public interface IAuthCodeStore
{
    string Store(string refreshToken);
    string? Consume(string code);
}

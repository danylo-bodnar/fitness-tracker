using FitnessTracker.Contracts.Dtos;

namespace FitnessTracker.Application.Common.Interfaces;

public interface ILoginEventPublisher
{
    Task PublishApprovedAsync(string nonce, string jwt, UserDto user, CancellationToken ct);
}

namespace FitnessTracker.Contracts.Dtos;

public record TelegramLoginResultDto(
    string AccessToken,
    UserDto User,
    string? RefreshToken = null
);

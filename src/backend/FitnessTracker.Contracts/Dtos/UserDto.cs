namespace FitnessTracker.Contracts.Dtos;

public record UserDto(
    Guid Id,
    long TelegramChatId,
    string TelegramUsername,
    string Role
);

namespace FitnessTracker.Contracts.Dtos;

public record UserDto(
    long TelegramChatId,
    string DisplayName
);

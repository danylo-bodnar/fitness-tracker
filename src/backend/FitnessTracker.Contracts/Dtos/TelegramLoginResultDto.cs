namespace FitnessTracker.Contracts.Dtos;

public record TelegramLoginResultDto(
    string Jwt,
    UserDto User
);

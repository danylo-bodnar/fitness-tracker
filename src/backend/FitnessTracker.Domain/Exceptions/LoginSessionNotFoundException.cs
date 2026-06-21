namespace FitnessTracker.Domain.Exceptions;

public class LoginSessionNotFoundException(string nonce)
    : DomainException($"Login session '{nonce}' not found.");

public class LoginSessionExpiredException(string nonce)
    : DomainException($"Login session '{nonce}' has expired.");

public class LoginSessionAlreadyUsedException(string nonce)
    : DomainException($"Login session '{nonce}' was already used.");

public class UserNotFoundException(long telegramChatId)
    : DomainException($"User with Telegram Chat ID '{telegramChatId}' not found.");
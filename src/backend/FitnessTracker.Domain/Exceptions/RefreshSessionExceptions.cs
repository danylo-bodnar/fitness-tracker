namespace FitnessTracker.Domain.Exceptions;

public sealed class RefreshSessionNotFoundException()
    : DomainException("Refresh session not found.");

public sealed class RefreshSessionInvalidException()
    : DomainException("Refresh session is invalid.");

public sealed class RefreshTokenReuseException(string token)
    : DomainException($"Revoked refresh token was reused: {token}. Possible token theft.");

public sealed class RefreshSessionAlreadyRevokedException(string token)
    : DomainException($"Refresh session {token} is already revoked.");

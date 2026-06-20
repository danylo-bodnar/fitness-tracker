using MediatR;

namespace FitnessTracker.Application.Auth.Commands;

public record SseEvent(string EventType, object Data);

public record StreamTelegramLoginQuery(string Nonce) : IStreamRequest<SseEvent>;

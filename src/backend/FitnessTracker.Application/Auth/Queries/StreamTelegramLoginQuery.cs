using MediatR;

namespace FitnessTracker.Application.Auth.Queries;

public record SseEvent(string EventType, object Data);

public record StreamTelegramLoginQuery(string Nonce) : IStreamRequest<SseEvent>;

namespace FitnessTracker.Application.Common.Interfaces;

public interface ILoginSessionNotifier
{
    void NotifyChanged(string nonce);
    Task WaitForChangeAsync(string nonce, CancellationToken ct);
    void CancelWait(string nonce);
}

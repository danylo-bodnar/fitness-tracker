using System.Collections.Concurrent;
using FitnessTracker.Application.Common.Interfaces;

namespace FitnessTracker.Infrastructure.Services;

public sealed class LoginSessionNotifier : ILoginSessionNotifier
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource> _waiters = new();

    public Task WaitForChangeAsync(string nonce, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        if (!_waiters.TryAdd(nonce, tcs))
            throw new InvalidOperationException($"A waiter for nonce '{nonce}' is already registered.");

        var reg = ct.Register(() =>
        {
            if (_waiters.TryRemove(nonce, out var t))
                t.TrySetCanceled(ct);
        });

        tcs.Task.ContinueWith(_ => reg.Dispose(), TaskScheduler.Default);
        return tcs.Task;
    }

    public void NotifyChanged(string nonce)
    {
        if (_waiters.TryRemove(nonce, out var tcs))
            tcs.TrySetResult();
    }

    public void CancelWait(string nonce)
    {
        if (_waiters.TryRemove(nonce, out var tcs))
            tcs.TrySetResult();
    }
}

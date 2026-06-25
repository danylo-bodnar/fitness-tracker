namespace FitnessTracker.Application.Common.Utilities;

public static class TaskExtensions
{
    public static async Task<bool> WaitUntilAsync(this Task task, DateTime deadline, CancellationToken ct)
    {
        var timeout = deadline - DateTime.UtcNow;
        if (timeout <= TimeSpan.Zero) return true;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeoutCts.CancelAfter(timeout);
        try
        {
            await task.WaitAsync(timeoutCts.Token);
            return false;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return true;
        }
    }
}

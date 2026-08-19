using System.Collections.Concurrent;
using NetCord.Services.ApplicationCommands;

namespace PrometheusBot.Services;

/// <summary>
/// Keeps HTTP requests ‘open’ until the corresponding Discord button click
/// (verify_confirm / verify_deny) for the relevant TokenId is received,
/// or until the timeout expires. A TokenId can only ever be
/// registered once at a time.
/// </summary>
public class PendingVerificationService
{
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<bool>> _pending = new();
    
    /// <summary>
    /// Registers a new pending request for the given token.
    /// Throws an InvalidOperationException if a request is already pending
    /// for this token (e.g. a retry from the backend following a network timeout).
    /// </summary>
    public Task<bool> WaitForResultAsync(Guid tokenId, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_pending.TryAdd(tokenId, tcs))
            throw new InvalidOperationException($"Für Token '{tokenId}' wird bereits eine Anfrage verarbeitet.");
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        cts.Token.Register(() =>
        {
            if (_pending.TryRemove(tokenId, out var pendingTcs))
                pendingTcs.TrySetCanceled();
        });
        _ = tcs.Task.ContinueWith(
           _ => cts.Dispose(), 
           CancellationToken.None, 
           TaskContinuationOptions.ExecuteSynchronously,
           TaskScheduler.Default); 
        
        return tcs.Task;
    }

    /// <summary>
    /// Resolves a pending request. Called from the Discord interaction handler
    /// as soon as the user clicks ‘Confirm’ or ‘Reject’.
    /// Returns false if there is no matching request (anymore)
    /// (e.g. the request has already expired).
    /// </summary>
    public bool TryResolve(Guid tokenId, bool confirmed)
    {
        if (_pending.TryRemove(tokenId, out var tcs))
        {
            tcs.TrySetResult(confirmed);
            return true;
        }
        return false;
    }
}
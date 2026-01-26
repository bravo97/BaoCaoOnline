using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public static class RetryHelper
    {
        public static async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxRetries = 3, int delayMs = 200, Func<Exception, bool>? shouldRetry = null, CancellationToken cancellationToken = default)
        {
            int attempt = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    return await action();
                }
                catch (Exception ex) when (attempt < maxRetries && (shouldRetry == null || shouldRetry(ex)))
                {
                    attempt++;
                    var backoff = delayMs * (int)Math.Pow(2, attempt - 1);
                    try
                    {
                        await Task.Delay(backoff, cancellationToken);
                    }
                    catch (OperationCanceledException) { throw; }
                }
            }
        }

        public static async Task RetryAsync(Func<Task> action, int maxRetries = 3, int delayMs = 200, Func<Exception, bool>? shouldRetry = null, CancellationToken cancellationToken = default)
        {
            await RetryAsync<object>(async () => { await action(); return null!; }, maxRetries, delayMs, shouldRetry, cancellationToken);
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices
{
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueTaskAsync(IScopedBackgroundTask workItem);

        ValueTask<IScopedBackgroundTask> DequeueTaskAsync(CancellationToken cancellationToken);

        int Count();
    }
}

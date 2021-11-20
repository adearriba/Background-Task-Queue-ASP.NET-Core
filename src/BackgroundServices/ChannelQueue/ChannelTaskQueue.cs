using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BackgroundServices.ChannelQueue
{
    public class ChannelTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<IScopedBackgroundTask> _queue;

        public ChannelTaskQueue(int? capacity)
        {
            if (capacity is null)
            {
                _queue = Channel.CreateUnbounded<IScopedBackgroundTask>();
            }
            else
            {
                var options = new BoundedChannelOptions(capacity.Value)
                {
                    FullMode = BoundedChannelFullMode.Wait
                };
                _queue = Channel.CreateBounded<IScopedBackgroundTask>(options);
            }
        }

        public async ValueTask<IScopedBackgroundTask> DequeueTaskAsync(CancellationToken cancellationToken)
        {
            var workItem = await _queue.Reader.ReadAsync(cancellationToken);
            return workItem;
        }

        public async ValueTask QueueTaskAsync(IScopedBackgroundTask workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }
    }
}

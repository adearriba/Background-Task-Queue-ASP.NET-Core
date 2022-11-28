using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BackgroundServices.ChannelQueue
{
    public class ChannelTaskQueue : IBackgroundTaskQueue
    {
        protected readonly Channel<IScopedBackgroundTask> _queue;

        public ChannelTaskQueue()
        {
            _queue = Channel.CreateUnbounded<IScopedBackgroundTask>();
        }

        public ChannelTaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<IScopedBackgroundTask>(options);
        }

        public int Count()
        {
            if(_queue.Reader.CanCount)
            {
                return _queue.Reader.Count;
            }

            return 0;
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

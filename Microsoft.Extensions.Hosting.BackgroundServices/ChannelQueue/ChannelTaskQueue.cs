using Microsoft.Extensions.Hosting.BackgroundServices.ChannelQueue.Generics;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;

namespace Microsoft.Extensions.Hosting.BackgroundServices.ChannelQueue
{
    public class ChannelTaskQueue : ITaskQueue<IBackgroundTask>
    {
        private readonly Channel<IBackgroundTask> _channel;
        private readonly ILogger<ChannelTaskQueue<IBackgroundTask>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;

        public ChannelTaskQueue(Channel<IBackgroundTask> channel,
            ILogger<ChannelTaskQueue<IBackgroundTask>> logger,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime)
        {
            _channel = channel;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void QueueTask(IBackgroundTask task)
        {
            if (!_cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"[TaskId: {task.Guid}] Adding {typeof(IBackgroundTask).Name} task to the queue.");

                task.ServiceProvider = _serviceProvider;
                _channel.Writer.WriteAsync(task);
            }
        }
    }
}

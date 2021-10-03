using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;

namespace Microsoft.Extensions.Hosting.BackgroundServices.ChannelQueue.Generics
{
    public class ChannelTaskQueue<T> : ITaskQueue<T> where T : IBackgroundTask
    {
        private readonly Channel<T> _channel;
        private readonly ILogger<ChannelTaskQueue<T>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;

        public ChannelTaskQueue(Channel<T> channel,
            ILogger<ChannelTaskQueue<T>> logger,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime applicationLifetime)
        {
            _channel = channel;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void QueueTask(T task)
        {
            if (!_cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"[TaskId: {task.Guid}] Adding {typeof(T).Name} task to the queue.");

                task.ServiceProvider = _serviceProvider;
                _channel.Writer.WriteAsync(task);
            }
        }
    }
}

using BackgroundServices.ChannelQueue.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices.ChannelQueue
{
    public class ChannelQueueHostedService : BackgroundService
    {
        private readonly ILogger<ChannelQueueHostedService> _logger;
        private ChannelTaskQueue _queue;

        public IServiceProvider Services { get; }

        public ChannelQueueHostedService(IServiceProvider services, 
            ChannelTaskQueue queue, ILogger<ChannelQueueHostedService> logger)
        {
            Services = services;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger?.ServiceStarting(nameof(ChannelQueueHostedService));
            await BackgroundProcessing(cancellationToken);
        }

        private async Task BackgroundProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var currentTask = await _queue.DequeueTaskAsync(cancellationToken);
                _logger?.TaskDequeue(currentTask);
                try
                {
                    using (var scope = Services.CreateScope())
                    {
                        _logger?.TaskStarting(currentTask);
                        await currentTask.DoWork(scope, cancellationToken);
                        _logger?.TaskEnd(currentTask);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.TaskError(currentTask, ex);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger?.ServiceEnd(nameof(ChannelQueueHostedService));
            await base.StopAsync(stoppingToken);
        }
    }
}

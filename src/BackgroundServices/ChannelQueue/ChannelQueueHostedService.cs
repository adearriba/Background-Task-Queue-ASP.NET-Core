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
        private IBackgroundTaskQueue _queue;
        private bool _isCleaning = false;

        public IServiceProvider Services { get; }

        public ChannelQueueHostedService(IServiceProvider services,
            IBackgroundTaskQueue queue, ILogger<ChannelQueueHostedService> logger)
        {
            Services = services;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger?.ServiceStarting(this.GetType().Name);
            await BackgroundProcessing(cancellationToken);
        }

        protected async Task BackgroundProcessing(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_isCleaning) await CleanQueue(cancellationToken);

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
            _logger?.ServiceEnd(this.GetType().Name);
            await base.StopAsync(stoppingToken);
        }

        private async Task CleanQueue(CancellationToken cancellationToken)
        {
            while(_queue.Count() > 0)
            {
                await _queue.DequeueTaskAsync(cancellationToken);
            }

            _logger?.TaskQueueCleaned(this.GetType().Name);
            _isCleaning = false;
        }

        public void StartCleaning()
        {
            _isCleaning = true;
        }
    }
}

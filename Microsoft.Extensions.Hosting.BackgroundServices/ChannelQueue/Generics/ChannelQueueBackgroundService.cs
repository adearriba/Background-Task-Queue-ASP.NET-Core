using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting.BackgroundServices.ChannelQueue.Generics
{
    public class ChannelQueueBackgroundService<T> : BackgroundService
        where T : IBackgroundTask
    {
        private readonly ILogger<ChannelQueueBackgroundService<T>> _logger;
        private Channel<T> _channel;

        public string ServiceName => GetType().Name;

        public ChannelQueueBackgroundService(
            Channel<T> channel,
            ILogger<ChannelQueueBackgroundService<T>> logger)
        {
            _channel = channel;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{ServiceName} background service is starting.");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTask = await _channel.Reader.ReadAsync(stoppingToken);

                try
                {
                    await currentTask.DoWork(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[TaskId: {currentTask.Guid}] Error occurred executing {currentTask.GetType().Name}.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{ServiceName} is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}

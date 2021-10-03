using Microsoft.Extensions.Hosting.BackgroundServices.ChannelQueue.Generics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting.BackgroundServices.ChannelQueue
{
    public class ChannelQueueBackgroundService : BackgroundService
    {
        private readonly ILogger<ChannelQueueBackgroundService<IBackgroundTask>> _logger;
        private Channel<IBackgroundTask> _channel;

        public string ServiceName => GetType().Name;

        public ChannelQueueBackgroundService(
            Channel<IBackgroundTask> channel,
            ILogger<ChannelQueueBackgroundService<IBackgroundTask>> logger)
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
                    _logger.LogTrace($"[TaskId: {currentTask.Guid}] Starting task...");
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

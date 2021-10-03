using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting.BackgroundServices
{
    public interface IBackgroundTask
    {
        public Guid Guid { get; }
        public IServiceProvider ServiceProvider { get; set; }

        Task DoWork(CancellationToken token = default);
    }
}

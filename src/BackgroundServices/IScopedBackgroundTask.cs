using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices
{
    public interface IScopedBackgroundTask : IDisposable
    {
        public Guid Guid { get; }

        ValueTask DoWork(IServiceScope scope, CancellationToken cancellationToken = default);
    }
}

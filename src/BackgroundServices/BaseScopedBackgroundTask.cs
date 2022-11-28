using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundServices
{
    public abstract class BaseScopedBackgroundTask : IScopedBackgroundTask
    {
        protected bool _disposed = false;
        protected readonly Guid _guid;


        public BaseScopedBackgroundTask()
        {
            _guid = Guid.NewGuid();
        }

        public Guid Guid => _guid;
        public void Dispose() => Dispose(true);

        protected abstract void Dispose(bool disposing);

        public abstract ValueTask DoWork(IServiceScope scope, CancellationToken cancellationToken);
    }
}

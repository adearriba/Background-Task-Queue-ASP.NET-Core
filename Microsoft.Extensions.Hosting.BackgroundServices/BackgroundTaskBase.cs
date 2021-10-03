using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting.BackgroundServices
{
    public abstract class BackgroundTaskBase : IBackgroundTask
    {
        protected readonly Guid _guid;
        protected IServiceProvider _serviceProvider;

        public BackgroundTaskBase()
        {
            _guid = Guid.NewGuid();
        }

        public Guid Guid => _guid;

        public IServiceProvider ServiceProvider
        {
            get => _serviceProvider;
            set => _serviceProvider = value;
        }

        public abstract Task DoWork(CancellationToken token = default);
    }
}

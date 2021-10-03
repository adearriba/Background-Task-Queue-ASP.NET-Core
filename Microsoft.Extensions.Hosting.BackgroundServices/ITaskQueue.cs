using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting.BackgroundServices
{
    public interface ITaskQueue<T> where T : IBackgroundTask
    {
        void QueueTask(T task);
    }
}

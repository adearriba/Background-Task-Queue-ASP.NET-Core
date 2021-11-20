using Microsoft.Extensions.Logging;
using System;

namespace BackgroundServices.ChannelQueue.Logging
{
    internal static class ChannelQueueHostedServiceLogging
    {
        internal static class EventIds
        {
            public static readonly EventId ServiceStarting = new EventId(100, "ServiceStarting");
            public static readonly EventId ServiceEnd = new EventId(100, "ServiceEnd");

            public static readonly EventId TaskDequeue = new EventId(201, "TaskDequeue");
            public static readonly EventId TaskStarting = new EventId(202, "TaskStarting");
            public static readonly EventId TaskEnd = new EventId(203, "TaskEnd");
            public static readonly EventId TaskError = new EventId(204, "TaskError");
        }

        private static readonly Action<ILogger, string, Exception> _taskDequeue = LoggerMessage.Define<string>(
                LogLevel.Trace,
                EventIds.TaskDequeue,
                "[Task: {taskId}] Task dequeued.");

        public static void TaskDequeue(this ILogger logger, IScopedBackgroundTask task)
        {
            _taskDequeue(logger, task.Guid.ToString(), null);
        }

        private static readonly Action<ILogger, string, Exception> _taskStarting = LoggerMessage.Define<string>(
                LogLevel.Trace,
                EventIds.TaskStarting,
                "[Task: {taskId}] Task is starting.");

        public static void TaskStarting(this ILogger logger, IScopedBackgroundTask task)
        {
            _taskStarting(logger, task.Guid.ToString(), null);
        }

        private static readonly Action<ILogger, string, Exception> _taskEnd = LoggerMessage.Define<string>(
                LogLevel.Trace,
                EventIds.TaskEnd,
                "[Task: {taskId}] Task ended correctly.");

        public static void TaskEnd(this ILogger logger, IScopedBackgroundTask task)
        {
            _taskEnd(logger, task.Guid.ToString(), null);
        }

        private static readonly Action<ILogger, string, Exception> _taskError = LoggerMessage.Define<string>(
                LogLevel.Error,
                EventIds.TaskError,
                "[Task: {taskId}] Error occurred executing task.");

        public static void TaskError(this ILogger logger, IScopedBackgroundTask task, Exception exception)
        {
            _taskError(logger, task.Guid.ToString(), exception);
        }

        private static readonly Action<ILogger, string, Exception> _serviceStarting = LoggerMessage.Define<string>(
                LogLevel.Trace,
                EventIds.ServiceStarting,
                "Background Service {backgroundServiceName} is starting...");

        public static void ServiceStarting(this ILogger logger, string name)
        {
            _serviceStarting(logger, name, null);
        }

        private static readonly Action<ILogger, string, Exception> _serviceEnd = LoggerMessage.Define<string>(
                LogLevel.Trace,
                EventIds.ServiceEnd,
                "Background Service {backgroundServiceName} is stopping...");

        public static void ServiceEnd(this ILogger logger, string name)
        {
            _serviceEnd(logger, name, null);
        }
    }
}

using System;

namespace CJG.Infrastructure.NotificationService
{
    /// <summary>
    /// <typeparamref name="INotificationJob"/> interface, provides common interface for notification jobs.
    /// </summary>
    internal interface INotificationJob
    {
        SystemExitCode StartNotificationService(DateTime currentDate);
    }
}
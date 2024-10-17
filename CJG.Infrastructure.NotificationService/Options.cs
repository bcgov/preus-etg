using System;

namespace CJG.Infrastructure.NotificationService
{
    /// <summary>
    /// <typeparamref name="Options"/> class, provides configuration settings for the <typeparamref name="NotificationService"/>
    /// </summary>
    public sealed class Options
    {
        /// <summary>
        /// get/set - Whether the console window should remain open after completion.
        /// </summary>
        public bool PauseBeforeExit { get; set; }

        /// <summary>
        /// get/set - The current date is used for testing the application.
        /// </summary>
        public DateTime CurrentDate { get; set; }
    }
}

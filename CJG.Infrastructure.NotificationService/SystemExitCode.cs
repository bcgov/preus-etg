namespace CJG.Infrastructure.NotificationService
{
    /// <summary>
    /// <typeparamref name="SystemExitCode"/> enum, provides a list of Windows console exit codes.
    /// </summary>
    internal enum SystemExitCode
    {
        /// <summary>
        /// Success - The application successfully completed without errors.
        /// </summary>
        Success = 0,
        /// <summary>
        /// FatalError - The application threw an exception and did not complete.
        /// </summary>
        FatalError = 1
    }
}
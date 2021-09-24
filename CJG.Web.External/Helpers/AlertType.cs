namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="AlertType"/> enum, provides a list of alert types and controls how they are displayed in the View.
    /// </summary>
    public enum AlertType
    {
        /// <summary>
        /// Default - Blue background.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Success - Green background.
        /// </summary>
        Success = 1,
        /// <summary>
        /// Warning - Orange background.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Error - Red background.
        /// </summary>
        Error = 3
    }
}
namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantOpeningStates"/> enum, provides a way to manage the current state of a <typeparamref name="GrantOpening"/>.
    /// </summary>
    public enum GrantOpeningStates
    {
        /// <summary>
        /// The Grant Opening is unscheduled.
        /// </summary>
        Unscheduled = 0,
        /// <summary>
        /// The Grant Opening is scheduled.
        /// </summary>
        Scheduled = 1,
        /// <summary>
        /// The Grant Opening is published and Grant Applications can be made for it.
        /// </summary>
        Published = 2,
        /// <summary>
        /// The Grant Opening is open and Grant Applications can now be submitted for it.
        /// </summary>
        Open = 3,
        /// <summary>
        /// The Grant Opening is closed and will no longer accept Grant Applications.
        /// </summary>
        Closed = 4,
        /// <summary>
        /// The Grant Opening is open only for submitting InProgress GrantApplications.
        /// </summary>
        OpenForSubmit = 5
    }
}

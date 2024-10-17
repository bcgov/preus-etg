namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="TrainingProviderStates"/> enum, provides all the states for training providers.
    /// </summary>
    public enum TrainingProviderStates
    {
        /// <summary>
        /// Incomplete - The information related to training providers is incomplete.
        /// </summary>
        Incomplete = 0,
        /// <summary>
        /// Complete - The information related to training providers is complete and can be submitted.
        /// </summary>
        Complete = 1,
        /// <summary>
        /// Requested - When a change request is submitted this provides a way to identify which one has been requested.
        /// </summary>
        Requested = 2,
        /// <summary>
        /// RequestApproved - The training provider request has been approved.
        /// </summary>
        RequestApproved = 3,
        /// <summary>
        /// RequestDenies - The training provider request has been denied.
        /// </summary>
        RequestDenied = 4,
        /// <summary>
        /// Denied - The training provider has been denied.
        /// </summary>
        Denied = 5
    }
}

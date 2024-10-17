namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="TrainingCostState"/> enum, provides a list of valid states for training costs.
    /// </summary>
    public enum TrainingCostStates
    {
        /// <summary>
        /// Incomplete - The information related to training cost is incomplete.
        /// </summary>
        Incomplete = 0,
        /// <summary>
        /// Complete - The information related to training cost is complete and can be submitted.
        /// </summary>
        Complete = 1
    }
}

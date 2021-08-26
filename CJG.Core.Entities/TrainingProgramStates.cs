namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="TrainingProgramStates"/> enum, provides the possible states for training programs.
    /// </summary>
    public enum TrainingProgramStates
    {
        /// <summary>
        /// Incomplete - The information related to the training program is incomplete.
        /// </summary>
        Incomplete = 0,
        /// <summary>
        /// Complete - The information related to the training program is complete and can be submitted.
        /// </summary>
        Complete = 1
    }
}

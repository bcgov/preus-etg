namespace CJG.Core.Entities
{
    /// <summary>
    /// TrainingResult class, provides a way to manage a list of valid training result.
    /// </summary>
    public class TrainingResult : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="TrainingResult"/> object.
        /// </summary>
        public TrainingResult() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="TrainingResult"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public TrainingResult(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

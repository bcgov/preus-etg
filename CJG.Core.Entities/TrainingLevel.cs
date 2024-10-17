namespace CJG.Core.Entities
{
    /// <summary>
    /// TrainingLevel class, provides a way to manage a list of valid training level.
    /// </summary>
    /// <example>
    ///     Level 1
    ///     Level 2
    ///     Level 3
    ///     Level 4
    /// </example>
    public class TrainingLevel: LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="TrainingLevel"/> object.
        /// </summary>
        public TrainingLevel() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="TrainingLevel"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public TrainingLevel(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

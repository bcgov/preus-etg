namespace CJG.Core.Entities
{
    /// <summary>
    /// A CanadianStatus class, provides a way to manage a list of canadian statuses.
    /// </summary>
    public class CanadianStatus : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="CanadianStatus"/> object.
        /// </summary>
        public CanadianStatus() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="CanadianStatus"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public CanadianStatus(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

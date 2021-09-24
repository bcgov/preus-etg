namespace CJG.Core.Entities
{
    /// <summary>
    /// RecentPeriod class, provides a way to manage a list of valid recent period, these are used in the participant form.
    /// </summary>
    public class RecentPeriod : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="RecentPeriod"/> object.
        /// </summary>
        public RecentPeriod() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="RecentPeriod"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public RecentPeriod(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

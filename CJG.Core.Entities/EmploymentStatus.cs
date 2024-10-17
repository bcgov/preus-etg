namespace CJG.Core.Entities
{
    /// <summary>
    /// EmploymentStatus class, provides a way to manage a list of employment statuses.
    /// </summary>
    public class EmploymentStatus : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="EmploymentStatus"/> object.
        /// </summary>
        public EmploymentStatus() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="EmploymentStatus"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public EmploymentStatus(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

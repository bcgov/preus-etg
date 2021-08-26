namespace CJG.Core.Entities
{
    /// <summary>
    /// EmploymentType class, provides a way to manage a list of employment type.
    /// </summary>
    public class EmploymentType : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="EmploymentType"/> object.
        /// </summary>
        public EmploymentType() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="EmploymentType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public EmploymentType(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

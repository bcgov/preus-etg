namespace CJG.Core.Entities
{
    /// <summary>
    /// ExpectedQualification class, provides a way to manage a list of expected qualifications.
    /// </summary>
    /// <example>
    ///     Educational
    ///     Industry/Occupation(less than 10 hours)
    ///     Industry/Occupation(more than 10 hours)
    ///     Proprietary(firm issued)
    /// </example>
    public class ExpectedQualification : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="ExpectedQualification"/> object.
        /// </summary>
        public ExpectedQualification() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="ExpectedQualification"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public ExpectedQualification(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

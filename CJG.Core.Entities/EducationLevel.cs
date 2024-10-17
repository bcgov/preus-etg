namespace CJG.Core.Entities
{
    /// <summary>
    /// A EducationLevel class, provides a way to manage a list of education levels.
    /// </summary>
    public class EducationLevel : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="EducationLevel"/> object.
        /// </summary>
        public EducationLevel() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="EducationLevel"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public EducationLevel(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

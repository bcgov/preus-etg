namespace CJG.Core.Entities
{
    /// <summary>
    /// UnderRepresentedGroup class, provides a way to manage a list of valid underrepresented group.
    /// </summary>
    public class UnderRepresentedGroup : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="UnderRepresentedGroup"/> object.
        /// </summary>
        public UnderRepresentedGroup() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="UnderRepresentedGroup"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public UnderRepresentedGroup(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

namespace CJG.Core.Entities
{
    /// <summary>
    /// ApplicationType class, provides a way to manage a list of valid application type.
    /// </summary>
    public class ApplicationType : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="ApplicationType"/> object.
        /// </summary>
        public ApplicationType() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="ApplicationType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public ApplicationType(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

namespace CJG.Core.Entities
{
    /// <summary>
    /// VulnerableGroup class, provides a way manage a list of valid vulnerable group.
    /// </summary>
    public class VulnerableGroup : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="VulnerableGroup"/> object.
        /// </summary>
        public VulnerableGroup() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="VulnerableGroup"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public VulnerableGroup(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

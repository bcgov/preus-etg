namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="AboriginalBand"/> class, provides the ORM a way to manage aboriginal band lookup values.
    /// </summary>
    public class AboriginalBand : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="AboriginalBand"/> object.
        /// </summary>
        public AboriginalBand() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="AboriginalBand"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public AboriginalBand(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {
            
        }
        #endregion
    }
}

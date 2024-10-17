namespace CJG.Core.Entities
{
    /// <summary>
    /// LegalStructure class, provides a way to manage all legal structure.
    /// </summary>
    /// <example>
    ///     Sole Proprietorship
    ///     Band Council
    ///     Partnership
    ///     British Columbia Corporation
    ///     Federal Corporation
    ///     Society
    ///     Unincorporated Association
    ///     British Columbia Cooperative
    ///     Federal Cooperative
    ///     Other
    /// </example>
    public class LegalStructure : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="LegalStructure"/> object.
        /// </summary>
        public LegalStructure() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="LegalStructure"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public LegalStructure(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

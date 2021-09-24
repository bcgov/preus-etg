namespace CJG.Core.Entities
{
    /// <summary>
    /// OrganizationType class, provides a list of valid organization types.
    /// </summary>
    /// <example>
    ///     Entry Level Skills
    ///     Upskilling or Upgrading
    ///     Maintenance for Current Job
    /// </example>
    public class OrganizationType : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="OrganizationType"/> object.
        /// </summary>
        public OrganizationType() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="OrganizationType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public OrganizationType(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

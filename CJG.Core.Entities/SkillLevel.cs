namespace CJG.Core.Entities
{
    /// <summary>
    /// SkillLevel class, provides a way to manage a list of valid skill level.
    /// </summary>
    /// <example>
    ///     Entry Level Skills
    ///     Upskilling or Upgrading
    ///     Maintenance for Current Job
    /// </example>
    public class SkillLevel : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="SkillLevel"/> object.
        /// </summary>
        public SkillLevel() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="SkillLevel"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public SkillLevel(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

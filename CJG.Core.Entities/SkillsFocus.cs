namespace CJG.Core.Entities
{
    /// <summary>
    /// SkillsFocus class, provides a way to manage a list of valid skills focus.
    /// </summary>
    /// <example>
    ///     Specialized or Technical Skills
    ///     Management or Business Skills
    ///     Essential Skills
    ///     Soft Skills
    ///     Apprenticeship Training - Also show in demand occupations and level of training.
    ///     Foundation Program - Also show in demand occupations.
    /// </example>
    public class SkillsFocus : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="SkillsFocus"/> object.
        /// </summary>
        public SkillsFocus() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="SkillsFocus"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public SkillsFocus(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

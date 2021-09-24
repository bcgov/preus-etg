namespace CJG.Core.Entities
{
    /// <summary>
    /// An EIBenefit class, provides a way to manage a list of EI Benefits.
    /// </summary>
    public class EIBenefit : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="EIBenefit"/> object.
        /// </summary>
        public EIBenefit() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="EIBenefit"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public EIBenefit(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}
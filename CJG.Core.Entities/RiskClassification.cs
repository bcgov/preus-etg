namespace CJG.Core.Entities
{
    /// <summary>
    /// RiskClassification class, provides a way to manage a list of risk classification.
    /// </summary>
    public class RiskClassification : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="RiskClassification"/> object.
        /// </summary>
        public RiskClassification() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="RiskClassification"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public RiskClassification(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

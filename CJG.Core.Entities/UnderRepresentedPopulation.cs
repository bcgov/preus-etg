using System.Collections.Generic;

namespace CJG.Core.Entities
{
    /// <summary>
    /// UnderRepresentedPopulation class, provides a way to manage a list of valid underrepresented population.
    /// </summary>
    public class UnderRepresentedPopulation : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="UnderRepresentedPopulation"/> object.
        /// </summary>
        public UnderRepresentedPopulation() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="UnderRepresentedPopulation"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public UnderRepresentedPopulation(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

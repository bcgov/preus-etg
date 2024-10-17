using System.Collections.Generic;

namespace CJG.Core.Entities
{
    /// <summary>
    /// PrioritySector class, provides a way to manage a list of valid priority sector.
    /// </summary>
    /// <example>
    ///     Aboriginal Peoples and First Nations
    ///     Agrifoods
    ///     Construction
    ///     Forestry
    ///     In Demand Organizations
    ///     Manufacturing
    ///     Mining and Energy
    ///     Natural Gas
    ///     Small Business
    ///     Technology and Green Economy
    ///     Tourism
    ///     Transportation
    /// </example>
    public class PrioritySector : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="PrioritySector"/> object.
        /// </summary>
        public PrioritySector() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="PrioritySector"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public PrioritySector(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

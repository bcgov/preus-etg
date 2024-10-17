namespace CJG.Core.Entities
{
    /// <summary>
    /// InDemandOccupation class, provides a way to manage a list of indemand occupations.
    /// </summary>
    /// <example>
    ///     Carpenters
    ///     Gasfitters
    ///     Electricians
    ///     Heavy Duty Equipment Mechanic
    ///     Heavy Equipment Operators
    ///     Machinists
    ///     Millwrights
    ///     Plumbers
    ///     Sheet Metal workers
    ///     Steamfitters, pipefitters, and sprinkler system installers
    ///     Painters and Decorators
    ///     Plasterers, drywall installers, and finishers and lathers
    ///     Industrial Electricians
    ///     Crane operators
    ///     Concrete Finishers
    ///     Cooks
    ///     Bakers
    ///     Welders
    /// </example>
    public class InDemandOccupation : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="InDemandOccupation"/> object.
        /// </summary>
        public InDemandOccupation() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="InDemandOccupation"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public InDemandOccupation(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}

namespace CJG.Core.Entities
{
    /// <summary>
    /// A DeliveryMethod class, provides a way to manage a list of delivery methods.
    /// </summary>
    /// <example>
    ///     Classroom
    ///     Workplace
    ///     Online
    /// </example>
    public class DeliveryMethod : LookupTable<int>
    {
        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="DeliveryMethod"/> object.
        /// </summary>
        public DeliveryMethod() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="DeliveryMethod"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="rowSequence"></param>
        public DeliveryMethod(string caption, int rowSequence = 0) : base(caption, rowSequence)
        {

        }
        #endregion
    }
}
namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="FilterOperator"/> enum, provides a list of filter operators.
    /// </summary>
    public enum FilterOperator
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal,
        /// <summary>
        /// Not Equal
        /// </summary>
        NotEqual,
        /// <summary>
        /// Less Than
        /// </summary>
        LessThan,
        /// <summary>
        /// Less Than Equal To
        /// </summary>
        LessThanEqualTo,
        /// <summary>
        /// Greater Than
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Greater Than Equal To
        /// </summary>
        GreaterThanEqualTo,
        /// <summary>
        /// I
        /// </summary>
        In,
        /// <summary>
        /// Not In
        /// </summary>
        NotIn,
        /// <summary>
        /// Is Null
        /// </summary>
        IsNull,
        /// <summary>
        /// Is Not Null
        /// </summary>
        IsNotNull,
        /// <summary>
        /// Like
        /// </summary>
        Like,
        /// <summary>
        /// Between
        /// </summary>
        Between,
        /// <summary>
        /// Any
        /// </summary>
        Any
    }
}

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="AccountTypes"/> enum, provides a way to identify the type of user.
    /// </summary>
    public enum AccountTypes
    {
        /// <summary>
        /// External - An external user.
        /// </summary>
        External = 0,
        /// <summary>
        /// Internal - An internal user.
        /// </summary>
        Internal = 1,
        /// <summary>
        /// Test - An external test user.
        /// </summary>
        Test = 2
    }
}

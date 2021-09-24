namespace CJG.Core.Entities
{
    /// <summary>
    /// This is the account type code provided by BCeID.
    /// It tells us what type of user is logged in.
    /// </summary>
    public enum BCeIDAccountTypeCodes
    {
        /// <summary>
        /// Unset, empty or unused value.
        /// </summary>
        Void = 0,
        /// <summary>
        /// Basic BCeID account type.  
        /// </summary>
        Individual = 1,
        /// <summary>
        /// Personal BCeID accounts type.
        /// </summary>
        VerifiedIndividual = 2,
        /// <summary>
        /// Business BCeID account type.
        /// </summary>
        Business = 3,
        /// <summary>
        /// The Internal BC Government directory (IDIR).
        /// </summary>
        Internal = 4,
        /// <summary>
        /// Liquor Distribution Branch federated accounts.
        /// </summary>
        LDB = 5,
        /// <summary>
        /// EDS federated accounts.
        /// </summary>
        EDS = 6,
        /// <summary>
        /// THS accounts
        /// </summary>
        THS = 7
    }
}

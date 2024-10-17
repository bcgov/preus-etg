namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="AppClaimTypes"/> static class, provides a list of application specific claim types.
    /// </summary>
    public static class AppClaimTypes
    {
        /// <summary>
        /// Privilege - The Claims associated with privileges.
        /// </summary>
        public const string Privilege = "Privilege";

        /// <summary>
        /// UserId - The user's application ID.
        /// </summary>
        public const string UserId = "UserId";

        /// <summary>
        /// AccountType - The BCeID account Type.
        /// </summary>
        public const string AccountType = "AccountType";

        /// <summary>
        /// OrganizationAdministrator - The organization administrator.
        /// </summary>
        public const string OrganizationAdministrator = "OrganizationAdministrator";

        /// <summary>
        /// OrganizationId - The organization application ID.
        /// </summary>
        public const string OrganizationId = "OrganizationId";

        /// <summary>
        /// OrganizationName - The organization name the user belongs to.
        /// </summary>
        public const string OrganizationName = "OrganizationName";
    }
}

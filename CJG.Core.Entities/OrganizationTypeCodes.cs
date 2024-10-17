using System.ComponentModel;

namespace CJG.Core.Entities
{
    /// <summary>
    /// OrganizationTypeCodes enum, provides a list of valid employer type codes.
    /// </summary>
    public enum OrganizationTypeCodes
    {
        /// <summary>
        /// Default - The standard employer.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Private - Private employers.
        /// </summary>
        [Description("Profit (Private)")]
        Private = 1,
        /// <summary>
        /// NonProfie - Nonprovide employers/
        /// </summary>
        [Description("Non-Profit")]
        NonProfit = 2,
        /// <summary>
        /// Goverment
        /// </summary>
        [Description("Government/Public")]
        Government = 3
    }
}

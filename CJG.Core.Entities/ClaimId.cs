using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// A ClaimId class, provides a way to manage the primary keys for Claims.
    /// </summary>
    public class ClaimId : EntityBase
    {
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}

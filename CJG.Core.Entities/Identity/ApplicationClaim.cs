using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CJG.Core.Entities;

namespace CJG.Infrastructure.Identity
{
    /// <summary>
    /// Contain all available security privilege claims for the whole application. 
    /// This is essentially a static lookup table that provides mapping between roles and privileges.
    /// <remarks>
    /// Content of this table should always be synced with CJG.Infrastructure.Identity.Privilege 
    /// enumeration
    /// </remarks>
    /// </summary>
    public class ApplicationClaim : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string ClaimType { get; set; }

        [Required]
        public string ClaimValue { get; set; }
        
        public virtual ICollection<ApplicationRole> ApplicationRoles { get; set; }
        
    }
}

using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CJG.Infrastructure.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string name) : base(name)
        {
        }

        public virtual ICollection<ApplicationClaim> ApplicationClaims { get; set; } = new List<ApplicationClaim>();
    }
}

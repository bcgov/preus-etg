using System.Security.Claims;
using System.Threading.Tasks;
using CJG.Core.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace CJG.Infrastructure.Identity
{
    /// <summary>
    /// <typeparamref name="ApplicationUser"/> class, provides an EF entity for the Microsoft Identity user.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        #region Properties
        public int? InternalUserId { get; set; }
        public virtual InternalUser InternalUser { get; set; }
        public bool? Active { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref namme="ApplicationUser"/> object.
        /// </summary>
        public ApplicationUser()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref namme="ApplicationUser"/> object.
        /// </summary>
        /// <param name="user"></param>
        public ApplicationUser(InternalUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            this.Id = Guid.NewGuid().ToString();
            this.InternalUser = user;
            this.InternalUserId = user.Id;
            this.Active = true;
            this.UserName = user.IDIR;
            this.Email = user.Email;
        }
        #endregion

        #region Methods
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            return userIdentity;
        }
        #endregion
    }
}

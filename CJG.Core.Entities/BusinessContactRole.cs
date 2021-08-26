using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="BusinessContactRole"/> class, provides the ORM a way to manage business contact roles.
    /// A business contact role provides a way to define the Application Administrator to a Grant Application.
    /// </summary>
    public class BusinessContactRole : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - the foreign key to the external user.
        /// </summary>
        [Required]
        [Index("IX_BusinessContactRole", 1, IsUnique = true)]
        public int UserId { get; set; }

        /// <summary>
        /// get/set - The external user.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        /// <summary>
        /// get/set - The foreign key to the parent grant application.
        /// </summary>
        [Required]
        [Index("IX_BusinessContactRole", 2, IsUnique = true)]
        public int GrantApplicationId { get; set; }

        /// <summary>
        /// get/set - The parent grant application.
        /// </summary>
        [ForeignKey(nameof(GrantApplicationId))]
        public GrantApplication GrantApplication { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="BusinessContactRole"/> object.
        /// </summary>
        public BusinessContactRole()
        { }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="BusinessContactRole"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="grantApplication"></param>
        /// <param name="applicationAdministrator"></param>
        public BusinessContactRole(GrantApplication grantApplication, User applicationAdministrator)
        {
            this.User = applicationAdministrator ?? throw new ArgumentNullException(nameof(applicationAdministrator));
            this.UserId = applicationAdministrator.Id;
            this.GrantApplication = grantApplication ?? throw new ArgumentNullException(nameof(grantApplication));
            this.GrantApplicationId = grantApplication.Id;
        }
        #endregion
    }
}
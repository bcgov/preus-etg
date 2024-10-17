using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CJG.Core.Entities
{
    /// <summary>
    /// UserGrantProgramPreference class, provides a way to maintain user preferences related to grant programs.
    /// </summary>
    public class UserGrantProgramPreference : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and foreign key to the parent user.
        /// </summary>
        [Key, Column(Order = 1), Index("IX_UserGrantProgramPreference", 1)]
        public int UserId { get; set; }

        /// <summary>
        /// get/set - The parent user.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        /// <summary>
        /// get/set - The primary key and foreign key to the parent grant program.
        /// </summary>
        [Key, Column(Order = 2), Index("IX_UserGrantProgramPreference", 2)]
        public int GrantProgramId { get; set; }

        /// <summary>
        /// get/set - The parent grant program.
        /// </summary>
        [ForeignKey(nameof(GrantProgramId))]
        public virtual GrantProgram GrantProgram { get; set; }

        /// <summary>
        /// get/set - Whether the grant program is selected by the user.
        /// </summary>
        [Index("IX_UserGrantProgramPreference", 3)]
        public bool IsSelected { get; set; } = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a UserGrantProgramPreference object.
        /// </summary>
        public UserGrantProgramPreference()
        {

        }

        /// <summary>
        /// Creates a new instance of a UserGrantProgramPreference object and initializes it.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="grantProgram"></param>
        public UserGrantProgramPreference(User user, GrantProgram grantProgram)
        {
            this.User = user ?? throw new ArgumentNullException(nameof(user));
            this.UserId = user.Id;
            this.GrantProgram = grantProgram ?? throw new ArgumentNullException(nameof(grantProgram));
            this.GrantProgramId = grantProgram.Id;
        }
        #endregion
    }
}

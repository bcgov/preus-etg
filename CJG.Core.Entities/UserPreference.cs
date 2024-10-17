using CJG.Core.Entities.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// UserPreference class, provides a way to manage user preferences.
    /// This is a one-to-one relationship with the user.
    /// </summary>
    public class UserPreference : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key and foreign key to the parent user.
        /// </summary>
        [Key, Column(nameof(UserId)), ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// get/set - The parent user.
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        /// <summary>
        /// get/set - When the user updated their grant program preferences.
        /// </summary>
        [DateTimeKind(DateTimeKind.Utc)]
        [Column(TypeName = "DATETIME2")]
        public DateTime GrantProgramPreferencesUpdated { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a UserPreference object.
        /// </summary>
        public UserPreference()
        {

        }

        /// <summary>
        /// Creates a new instance of a UserPreference object and initializes it.
        /// </summary>
        /// <param name="user"></param>
        public UserPreference(User user)
        {
            this.User = user ?? throw new ArgumentNullException(nameof(user));
            this.UserId = user.Id;
        }
        #endregion
    }
}

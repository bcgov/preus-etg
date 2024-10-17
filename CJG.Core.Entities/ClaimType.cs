using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// Provides a way to control claim behaviour.
    /// </summary>
    public enum ClaimTypes
    {
        /// <summary>
        /// SingleAmendableClaim - This claim type ensures that there can only be one claim created, but it can have many versions.
        /// </summary>
        SingleAmendableClaim = 1,
        /// <summary>
        /// MultipleClaimsWithoutAmendments - This claim type ensures that there can be many claims, but they are only allowed a single version each.
        /// </summary>
        MultipleClaimsWithoutAmendments = 2
    }

    /// <summary>
    /// ClaimType class, provides a way to control claim behaviour.
    /// </summary>
    public class ClaimType : LookupTable<int>
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key does not use IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new ClaimTypes Id { get; set; }

        /// <summary>
        /// get/set - A description of this claim type.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// get/set - Control whether this claim type can support multiple versions.
        /// </summary>
        [DefaultValue(true)]
        public bool IsAmendable { get; set; } = true;
        
        /// <summary>
        /// get - All of the claim configurations that use this claim type.
        /// </summary>
        public virtual ICollection<ProgramConfiguration> ProgramConfigurations { get; set; } = new List<ProgramConfiguration>();

        /// <summary>
        /// get - All of the claims that use this claim type.
        /// </summary>
        public virtual ICollection<Claim> Claims { get; set; } = new List<Claim>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ClaimType object.
        /// </summary>
        public ClaimType()
        {

        }

        /// <summary>
        /// Creates a new instance of a ClaimType object and initializes it.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="isAmendable"></param>
        /// <param name="isActive"></param>
        public ClaimType(ClaimTypes type, bool isAmendable = true, bool isActive = true) : base(type.GetDescription())
        {
            this.Id = type;
            this.IsAmendable = isAmendable;
            this.IsActive = isActive;
        }
        #endregion
    }
}

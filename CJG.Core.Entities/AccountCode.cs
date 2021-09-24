using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// AccountCode class, provides a way to maintain account codes for grant programs and grant stream.
    /// </summary>
    public class AccountCode : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key uses IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// get/set - The general ledger client number.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger client number is required."), MaxLength(50)]
        public string GLClientNumber { get; set; }

        /// <summary>
        /// get/set - The general ledger RESP account.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger RESP account is required."), MaxLength(20)]
        public string GLRESP { get; set; }

        /// <summary>
        /// get/set - The general ledger service line account.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger service line account is required."), MaxLength(20)]
        public string GLServiceLine { get; set; }

        /// <summary>
        /// get/set - The general ledger STOB normal account.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger STOB normal account is required."), MaxLength(20)]
        public string GLSTOBNormal { get; set; }

        /// <summary>
        /// get/set - The general ledger STOB accrual account.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger STOB accrual account is required."), MaxLength(20)]
        public string GLSTOBAccrual { get; set; }

        /// <summary>
        /// get/set - The general ledger project code.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The general ledger project code is required."), MaxLength(20)]
        public string GLProjectCode { get; set; }

        /// <summary>
        /// get - All the grant programs associated with this account code.
        /// </summary>
        public virtual ICollection<GrantProgram> GrantPrograms { get; set; } = new List<GrantProgram>();

        /// <summary>
        /// get - All the grant streams associated with this account code.
        /// </summary>
        public virtual ICollection<GrantStream> GrantStreams { get; set; } = new List<GrantStream>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of an AccountCode object.
        /// </summary>
        public AccountCode()
        {

        }
        #endregion
    }
}

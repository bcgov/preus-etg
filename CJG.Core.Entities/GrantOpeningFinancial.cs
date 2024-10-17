using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantOpeningFinancial"/> class, provides the ORM a way to manage the grant opening financial statements.
    /// This is a one-to-one relationship with the <typeparamref name="GrantOpening"/>.
    /// </summary>
    public class GrantOpeningFinancial : EntityBase
    {
        #region Properties
        /// <summary>
        /// get/set - Primary key and foreign key, which is the Grant Opening Id (one-to-one).
        /// </summary>
        [Key, Column(nameof(GrantOpeningId)), ForeignKey(nameof(GrantOpening))]
        public int GrantOpeningId { get; set; }

        /// <summary>
        /// get/set - The <typeparamref name="GrantOpening"/> associated with this financial statement.
        /// </summary>
        public virtual GrantOpening GrantOpening { get; set; }

        /// <summary>
        /// get/set - The total current reservations of funds for applications under assessment and not yet approved or denied for this Grant Opening. 
        /// </summary>
        public decimal CurrentReservations { get; set; }

        /// <summary>
        /// get/set - The sum of Assessed Commitments for applications-agreements linked to a Grant Opening. 
        /// </summary>
        public decimal AssessedCommitments { get; set; }

        /// <summary>
        /// get/set - Count of grant files that make up the original number of assessed commitments. 
        /// </summary>
        public int AssessedCommitmentsCount { get; set; }

        /// <summary>
        /// get/set - The total of outstanding commitments for which no claims have been received. Outstanding Commitments is shown on the Claims Management dashboard.  
        /// </summary>
        public decimal OutstandingCommitments { get; set; }

        /// <summary>
        /// get/set - The count of the grant files that make up current outstanding commitments. 
        /// </summary>
        public int OutstandingCommitmentCount { get; set; }

        /// <summary>
        /// get/set - The total value of canceled agreements that are linked to a Grant Opening. 
        /// </summary>
        public decimal Cancellations { get; set; }

        /// <summary>
        /// get/set - Count of grant files that are cancelled. 
        /// </summary>
        public int CancellationsCount { get; set; }

        /// <summary>
        /// get/set - The total value of claims assessed for a grant opening. 
        /// </summary>
        public decimal ClaimsAssessed { get; set; }

        /// <summary>
        /// get/set - The count of the grant files that have their claim assessed. 
        /// </summary>
        public int ClaimsAssessedCount { get; set; }

        /// <summary>
        /// get/set - The total current claims that result from the application and claim assessment workflow. Current Claims is shown on the Claims Management dashboard.  
        /// </summary>
        public decimal CurrentClaims { get; set; }

        /// <summary>
        /// get/set - The count of the grant files that have their claim received but not assessed. 
        /// </summary>
        public int CurrentClaimCount { get; set; }

        /// <summary>
        /// get/set - The total value of claims denied for a grant opening. 
        /// </summary>
        public decimal ClaimsDenied { get; set; }

        /// <summary>
        /// get/set - Count of the number of claim denied. 
        /// </summary>
        public int ClaimsDeniedCount { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantOpeningFinancial"/> object.
        /// </summary>
        public GrantOpeningFinancial()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantOpeningFinancial"/> object and initializes with the specified property values.
        /// </summary>
        /// <param name="grantOpening"></param>
        public GrantOpeningFinancial(GrantOpening grantOpening)
        {
            if (grantOpening == null)
                throw new ArgumentNullException(nameof(grantOpening));

            this.GrantOpening = grantOpening;
            this.GrantOpeningId = grantOpening.Id;
        }
        #endregion
    }
}

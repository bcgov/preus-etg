using DataAnnotationsExtensions;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantOpeningIntake"/> class, provides the ORM a way to manage the grant opening intake financial statements.
    /// This is a one-to-one relationship with the <typeparamref name="GrantOpening"/>.
    /// </summary>
    public class GrantOpeningIntake : EntityBase
    {
        #region Properties
        [Key, Column(nameof(GrantOpeningId)), ForeignKey(nameof(GrantOpening))]
        public int GrantOpeningId { get; set; }
        public virtual GrantOpening GrantOpening { get; set; }

        /// <summary>
        /// get/set - The number of grant applications that are in the New state.
        /// </summary>
        [Min(0, ErrorMessage = "The grant opening intake new count must be greater than or equal to 0."), DefaultValue(0)]
        public int NewCount { get; set; }
        [DefaultValue(0)]
        public decimal NewAmt { get; set; }

        /// <summary>
        /// get/set - The number of grant applications that are in the PendingAssessment state.
        /// </summary>
        [Min(0, ErrorMessage = "The grant opening intake pending assessment count must be greater than or equal to 0."), DefaultValue(0)]
        public int PendingAssessmentCount { get; set; }
        [DefaultValue(0)]
        public decimal PendingAssessmentAmt { get; set; }

        /// <summary>
        /// get/set - The number of grant applications that are in the UnderAssessment state.
        /// </summary>
        [Min(0, ErrorMessage = "The grant opening intake under assessment count must be greater than or equal to 0."), DefaultValue(0)]
        public int UnderAssessmentCount { get; set; }
        [DefaultValue(0)]
        public decimal UnderAssessmentAmt { get; set; }

        /// <summary>
        /// get/set - the number of grant applications that are in the ApplicationDenied state.
        /// </summary>
        [Min(0, ErrorMessage = "The grant opening intake denied count must be greater than or equal to 0."), DefaultValue(0)]
        public int DeniedCount { get; set; }
        [DefaultValue(0)]
        public decimal DeniedAmt { get; set; }

        /// <summary>
        /// get/set - The number of grant applications that are in the Application Withdrawn state.
        /// </summary>
        [Min(0, ErrorMessage = "The grant opening intake withdrawn count must be greater than or equal to 0."), DefaultValue(0)]
        public int WithdrawnCount { get; set; }
        [DefaultValue(0)]
        public decimal WithdrawnAmt { get; set; }

        [DefaultValue(0)]
        public decimal ReductionsAmt { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantOpeningIntake"/> object.
        /// </summary>
        public GrantOpeningIntake()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="GrantOpeningIntake"/> object and initializes with the specified property values.
        /// </summary>
        /// <param name="grantOpening"></param>
        public GrantOpeningIntake(GrantOpening grantOpening)
        {
            if (grantOpening == null)
                throw new ArgumentNullException(nameof(grantOpening));

            this.GrantOpening = grantOpening;
            this.GrantOpeningId = grantOpening.Id;
        }
        #endregion
    }
}

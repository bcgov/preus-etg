using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// A ServiceLine class, provides a way to configure eligible expense breakdowns for WDA Service program types.
    /// </summary>
    public class ServiceLine : LookupTable<int>
    {
        #region Properties
        /// <summary>
        /// get/set - A description for this service line.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// get/set - a caption used when presenting service lines as a breakdown.
        /// </summary>
        [MaxLength(250)]
        public string BreakdownCaption { get; set; }

        /// <summary>
        /// get/set - Foreign key to the parent service category.
        /// </summary>
        public int ServiceCategoryId { get; set; }

        /// <summary>
        /// get/set - The parent service category.
        /// </summary>
        [ForeignKey(nameof(ServiceCategoryId))]
        public virtual ServiceCategory ServiceCategory { get; set; }

        /// <summary>
        /// get/set - Whether to enable entering the cost during application development process.
        /// </summary>
        public bool EnableCost { get; set; }

        /// <summary>
        /// get - All the children service line breakdowns.
        /// </summary>
        public virtual ICollection<ServiceLineBreakdown> ServiceLineBreakdowns { get; set; } = new List<ServiceLineBreakdown>();

        /// <summary>
        /// get - All the related eligible expense breakdowns that are copies of this service line.
        /// </summary>
        public virtual ICollection<EligibleExpenseBreakdown> EligibleExpenseBreakdowns { get; set; } = new List<EligibleExpenseBreakdown>();

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ServiceLine class.
        /// </summary>
        public ServiceLine()
        {

        }

        /// <summary>
        /// Creates a new instance of a ServiceLine class and initializes it.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="breakdownCaption"></param>
        /// <param name="serviceCategoryId"></param>
        /// <param name="enableCost"></param>
        public ServiceLine(string caption, string breakdownCaption, int serviceCategoryId, bool enableCost = true) : base(caption)
        {
            this.BreakdownCaption = breakdownCaption;
            this.ServiceCategoryId = serviceCategoryId;
            this.EnableCost = enableCost;
        }
        #endregion
    }
}

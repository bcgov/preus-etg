using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// A ServiceCategory class, provides a way to manage all eligible expense types for the WDAService program type.
    /// </summary>
    public class ServiceCategory : LookupTable<int>
    {
        #region Properties
        /// <summary>
        /// get/set - A description of this claim type.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// get/set - The foreign key to the service type.
        /// </summary>
        public ServiceTypes ServiceTypeId { get; set; }

        /// <summary>
        /// get/set - The service type controls how this service category will affect the application process.
        /// </summary>
        [ForeignKey(nameof(ServiceTypeId))]
        public virtual ServiceType ServiceType { get; set; }

        /// <summary>
        /// get/set - The % rate to be applied to the eligible expense type.
        /// </summary>
        public double? Rate { get; set; }
        
        /// <summary>
        /// get/set - Whether to automatically include this as an eligible expense type.
        /// </summary>
        public bool AutoInclude { get; set; }

        /// <summary>
        /// get/set - Whether to allow multiple of this eligible expense type.
        /// </summary>
        public bool AllowMultiple { get; set; }

        /// <summary>
        /// get/set - The minimum number of training providers allowed to be associated with this eligible expense type.
        /// </summary>
        public int MinProviders { get; set; }

        /// <summary>
        /// get/set - The maximum number of training providers allowed to be associated with this eligible expense type.
        /// </summary>
        public int MaxProviders { get; set; }

        /// <summary>
        /// get/set - The minimum number of training programs allowed to be associated with this eligible expense type.
        /// </summary>
        public int MinPrograms { get; set; }

        /// <summary>
        /// get/set - The maximum number of training programs allowed to be associated with this eligible expense type.
        /// </summary>
        public int MaxPrograms { get; set; }

        /// <summary>
        /// get/set - Whether this eligible expense type must be associated with participants during completion reporting.
        /// </summary>
        public bool CompletionReport { get; set; }

        /// <summary>
        /// get - All the service lines related to this service category.
        /// </summary>
        public virtual ICollection<ServiceLine> ServiceLines { get; set; } = new List<ServiceLine>();

        /// <summary>
        /// get - All the eligible expense types associated with this service category.
        /// </summary>
        public virtual ICollection<EligibleExpenseType> EligibleExpenseTypes { get; set; } = new List<EligibleExpenseType>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ServiceCategory object.
        /// </summary>
        public ServiceCategory()
        {

        }
      
        /// <summary>
        /// Creates a new instance of a ServiceCategory object and initializes it.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="serviceType"></param>
        /// <param name="autoInclude"></param>
        /// <param name="allowMultiple"></param>
        /// <param name="minProviders"></param>
        /// <param name="maxProviders"></param>
        /// <param name="minPrograms"></param>
        /// <param name="maxPrograms"></param>
        /// <param name="completionReport"></param>
        public ServiceCategory(string caption, ServiceTypes serviceType, bool autoInclude = true, bool allowMultiple = false, int minProviders = 0, int maxProviders = 0, int minPrograms = 0, int maxPrograms = 0, bool completionReport = true) : base(caption)
        {
            this.ServiceTypeId = serviceType;
            this.AutoInclude = autoInclude;
            this.AllowMultiple = allowMultiple;
            this.MinProviders = minProviders;
            this.MaxProviders = maxProviders;
            this.MinPrograms = minPrograms;
            this.MaxPrograms = maxPrograms;
            this.CompletionReport = completionReport;
        }
        #endregion
    }
}

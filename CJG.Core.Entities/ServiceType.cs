using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CJG.Core.Entities
{
    /// <summary>
    /// Provides a way to control WDA service category behaviour during application development.
    /// </summary>
    public enum ServiceTypes
    {
        /// <summary>
        /// SkillsTraining - A service category that will allow multiple training programs to be added during application process.
        /// </summary>
        SkillsTraining = 1,
        /// <summary>
        /// EmploymentServicesAndSupports - A service category that will allow multiple training providers, or a service line selection option to be added during application process.
        /// </summary>
        EmploymentServicesAndSupports = 2,
        /// <summary>
        /// Administration - A service category that has no affect to the application process, it is only included in the total training costs.
        /// </summary>
        Administration = 3
    }

    /// <summary>
    /// ServiceType provides a way to control WDA service category behaviour during application development.
    /// </summary>
    public class ServiceType : LookupTable<int>
    {
        #region Properties
        /// <summary>
        /// get/set - The primary key does not use IDENTITY.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new ServiceTypes Id { get; set; }

        /// <summary>
        /// get/set - A description of this service type.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// get - All the service categories associated to this service type.
        /// </summary>
        public virtual ICollection<ServiceCategory> ServiceCategories { get; set; } = new List<ServiceCategory>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a ServiceType object.
        /// </summary>
        public ServiceType()
        {

        }

        /// <summary>
        /// Creates a new instance of a ServiceType object and initializes it.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isActive"></param>
        public ServiceType(ServiceTypes type, bool isActive = true) : base(type.GetDescription())
        {
            this.IsActive = isActive;
        }
        #endregion
    }
}

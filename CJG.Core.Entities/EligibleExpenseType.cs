using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CJG.Core.Entities
{
    /// <summary>
    /// EligibleExpenseType class, provides a way to manage eligible expense types.
    /// </summary>
    public class EligibleExpenseType : LookupTable<int>
    {
        #region  Properties
        /// <summary>
        /// get/set - The unique caption.
        /// </summary>
        [Required, MaxLength(250), Index("IX_EligibleExpenseType", Order = 2)]
        public new string Caption { get; set; }

        /// <summary>
        /// get/set - A description for this eligbible expense type.
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// get/set - Foreign key to the expense type.
        /// </summary>
        public ExpenseTypes ExpenseTypeId { get; set; }

        /// <summary>
        /// get/set - The expense type.
        /// </summary>
        [ForeignKey(nameof(ExpenseTypeId))]
        public virtual ExpenseType ExpenseType { get; set; }

        /// <summary>
        /// get/set - The rate that will be used for any automatic calculation.
        /// </summary>
        public double? Rate { get; set; }

        /// <summary>
        /// get/set - Whether to automatically include this eligible expense type in any training cost estimate.
        /// </summary>
        [DefaultValue(false)]
        public bool AutoInclude { get; set; }

        /// <summary>
        /// get/set - Whether to allow multiple training costs of this expense type.
        /// </summary>
        [DefaultValue(true)]
        public bool AllowMultiple { get; set; } = true;

        /// <summary>
        /// get/set - The foreign key to the service category.  This means it is copied from the WDAService master template.
        /// </summary>
        [Index("IX_EligibleExpenseType", Order = 1)]
        public int? ServiceCategoryId { get; set; }

        /// <summary>
        /// get/set - The service category this eligible expense type was copied from.
        /// </summary>
        [ForeignKey(nameof(ServiceCategoryId))]
        public virtual ServiceCategory ServiceCategory { get; set; }

        /// <summary>
        /// get/set - The minimum number of training providers that can be associated with this eligible expense type.
        /// </summary>
        [DefaultValue(0)]
        public int MinProviders { get; set; }

        /// <summary>
        /// get/set - The maximum number of training providers that can be associated with this eligible expense type.
        /// </summary>
        [DefaultValue(0)]
        public int MaxProviders { get; set; }

        /// <summary>
        /// get - All of the program configurations this expense type is associated with.
        /// </summary>
        public virtual ICollection<ProgramConfiguration> ProgramConfigurations { get; set; } = new List<ProgramConfiguration>();

        /// <summary>
        /// get - All of the breakdowns associated with this expense type.
        /// </summary>
        public virtual ICollection<EligibleExpenseBreakdown> Breakdowns { get; set; } = new List<EligibleExpenseBreakdown>();

        /// <summary>
        /// get - All of the claimed costs associated with this expense type.
        /// </summary>
        public virtual ICollection<ClaimEligibleCost> ClaimEligibleCosts { get; set; } = new List<ClaimEligibleCost>();

        /// <summary>
        /// get - All of the training cost associated with this expense type.
        /// </summary>
        public virtual ICollection<EligibleCost> EligibleCosts { get; set; } = new List<EligibleCost>();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <typeparamref name="EligibleExpenseType"/> object.
        /// </summary>
        public EligibleExpenseType()
        {

        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="EligibleExpenseType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="expenseType"></param>
        /// <param name="rowSequence"></param>
        public EligibleExpenseType(string caption, ExpenseType expenseType, int rowSequence = 0) : base(caption, rowSequence)
        {
            this.Caption = caption;
            if (expenseType == null) throw new ArgumentNullException(nameof(expenseType));
            this.ExpenseTypeId = (ExpenseTypes)expenseType.Id;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="EligibleExpenseType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="description"></param>
        /// <param name="expenseType"></param>
        /// <param name="rowSequence"></param>
        public EligibleExpenseType(string caption, string description, ExpenseTypes expenseType = ExpenseTypes.ParticipantAssigned, int rowSequence = 0) : base(caption, rowSequence)
        {
            this.Caption = caption;
            this.Description = description;
            this.ExpenseTypeId = expenseType;
        }

        /// <summary>
        /// Creates a new instance of a <typeparamref name="EligibleExpenseType"/> object and initializes it with the specified property values.
        /// </summary>
        /// <param name="serviceCategory"></param>
        /// <param name="expenseType"></param>
        public EligibleExpenseType(ServiceCategory serviceCategory, ExpenseTypes expenseType = ExpenseTypes.ParticipantAssigned) : this(serviceCategory?.Caption, serviceCategory?.Description, expenseType, serviceCategory?.RowSequence ?? 0)
        {
            this.ServiceCategoryId = serviceCategory?.Id ?? throw new ArgumentNullException(nameof(serviceCategory));
            this.ServiceCategory = serviceCategory;
            this.IsActive = serviceCategory.IsActive;
            this.MinProviders = serviceCategory.MinProviders;
            this.MaxProviders = serviceCategory.MaxProviders;
            this.AllowMultiple = serviceCategory.AllowMultiple;
            this.AutoInclude = serviceCategory.AutoInclude;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validates this eligible expense type before updating the datasource.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = validationContext.GetDbContext();
            var entry = validationContext.GetDbEntityEntry();

            // This is done to stop errors from being thrown when developers use EF entities in ViewModels.
            if (entry == null)
                yield break;

            if (this.ProgramConfigurations.Any(cc => cc.EligibleExpenseTypes.Any(eet => eet.Caption == this.Caption && eet.Id != this.Id)))
                yield return new ValidationResult("The caption must be unique within a program configuration.", new[] { nameof(this.Caption) });

            foreach (var validation in base.Validate(validationContext))
            {
                yield return validation;
            }
        }
        #endregion
    }
}

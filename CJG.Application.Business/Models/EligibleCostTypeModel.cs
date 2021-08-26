using CJG.Core.Entities;
using System;

namespace CJG.Application.Business.Models
{
    public class EligibleExpenseTypeModel : CollectionItemModel
    {
        #region Properties
        public double? Rate { get; set; }
        public bool AllowMultiple { get; set; }
        public bool AutoInclude { get; set; }
        public ExpenseTypes ExpenseTypeId { get; set; }

        public ServiceTypes? ServiceType { get; set; }
        #endregion  

        #region Constructors
        public EligibleExpenseTypeModel()
        { }

        public EligibleExpenseTypeModel(EligibleExpenseType eligibleExpenseType)
        {
            if (eligibleExpenseType == null)
                throw new ArgumentNullException(nameof(eligibleExpenseType));

            Id = eligibleExpenseType.Id;
            ExpenseTypeId = eligibleExpenseType.ExpenseTypeId;
            Caption = eligibleExpenseType.Caption;
            AllowMultiple = eligibleExpenseType.AllowMultiple;
            RowSequence = eligibleExpenseType.RowSequence;
            Rate = eligibleExpenseType.Rate;
            AutoInclude = eligibleExpenseType.AutoInclude;
            ServiceType = eligibleExpenseType.ServiceCategory?.ServiceTypeId;
        }
        #endregion
    }
}

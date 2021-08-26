using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{

    public interface IEligibleExpenseTypeService : IService
    {
        EligibleExpenseType Get(int id);
        EligibleExpenseType Add(EligibleExpenseType eligibleExpenseType);
        EligibleExpenseType Update(EligibleExpenseType eligibleExpenseType);
        void Delete(EligibleExpenseType eligibleExpenseType);
    }
}

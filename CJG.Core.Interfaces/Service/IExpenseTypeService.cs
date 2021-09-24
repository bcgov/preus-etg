using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IExpenseTypeService : IService
    {
        ExpenseType Get(ExpenseTypes id);
        IEnumerable<ExpenseType> GetAll();
        IEnumerable<ExpenseType> GetAll(bool isActive);
    }
}

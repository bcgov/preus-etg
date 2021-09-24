using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IEligibleCostService : IService
    {
        EligibleCost Get(int id);
        IEnumerable<EligibleCost> GetForGrantApplication(int grantApplicationId, ServiceTypes type);
        EligibleCost Add(EligibleCost eligibleCost);
        EligibleCost Update(EligibleCost eligibleCost);
        void Delete(EligibleCost eligibleCost);
    }
}

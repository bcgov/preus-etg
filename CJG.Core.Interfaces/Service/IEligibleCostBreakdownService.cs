using CJG.Application.Business.Models;
using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
   
    public interface IEligibleCostBreakdownService : IService
    {
        EligibleCostBreakdown Get(int id);
        EligibleCostBreakdown Add(EligibleCostBreakdown eligibleCostBreakdown);
        EligibleCostBreakdown Update(EligibleCostBreakdown eligibleCostBreakdown);
        void Delete(EligibleCostBreakdown eligibleCostBreakdown);
    }
}

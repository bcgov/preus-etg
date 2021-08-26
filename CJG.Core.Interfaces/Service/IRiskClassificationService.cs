using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IRiskClassificationService : IService
    {
        RiskClassification Get(int id);
        IEnumerable<RiskClassification> GetAll();
        IEnumerable<RiskClassification> GetAll(bool isActive);

        RiskClassification Add(RiskClassification riskClassification);
        RiskClassification Update(RiskClassification riskClassification);
        void Delete(RiskClassification riskClassification);
    }
}

using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IUnderRepresentedPopulationService : IService
    {
        IEnumerable<UnderRepresentedPopulation> GetUnderRepresentedPopulations(int[] ids);
    }
}

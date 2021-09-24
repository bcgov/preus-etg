using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
    public interface IVulnerableGroupService : IService
    {
        IEnumerable<VulnerableGroup> GetVulnerableGroups(int[] ids);
    }
}

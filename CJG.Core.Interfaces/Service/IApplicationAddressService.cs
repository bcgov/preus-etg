using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
    public interface IApplicationAddressService : IService
    {
        Region VerifyOrCreateRegion(string regionName, string addressCountryId);
        void RemoveAddressIfNotUsed(ApplicationAddress appAddress, int? excludeApplicationId = null);
    }
}

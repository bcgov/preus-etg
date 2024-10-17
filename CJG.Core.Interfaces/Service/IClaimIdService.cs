using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
    public interface IClaimIdService : IService
    {
        int AddClaimId(ClaimId newClaimId);
    }
}

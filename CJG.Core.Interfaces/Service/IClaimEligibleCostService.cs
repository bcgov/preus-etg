using System.Collections.Generic;
using CJG.Application.Business.Models;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IClaimEligibleCostService : IService
    {
        ClaimEligibleCost Get(int id);
        void Update(List<ClaimEligibleCostModel> claimEligibleCosts);
		void ResetClaimAmounts(Claim claim);
	}
}

using System;

namespace CJG.Core.Entities
{
    /// <summary>
    /// <typeparamref name="GrantOpeningExtensions"/> static class, provides extension methods for <typeparamref name="GrantOpening"/> objects.
    /// </summary>
    public static class GrantOpeningExtensions
    {
        /// <summary>
        /// Calculate the intake target for the grant opening based on the plan rates.
        /// </summary>
        /// <param name="grantOpening"></param>
        /// <returns>The grant opening intake target.</returns>
        public static decimal CalculateIntakeTarget(this GrantOpening grantOpening)
        {
            return grantOpening.BudgetAllocationAmt
                        + (Math.Round(grantOpening.BudgetAllocationAmt * (decimal)grantOpening.PlanDeniedRate, 0, MidpointRounding.AwayFromZero))
                        + (Math.Round(grantOpening.BudgetAllocationAmt * (decimal)grantOpening.PlanWithdrawnRate, 0, MidpointRounding.AwayFromZero))
                        + (Math.Round(grantOpening.BudgetAllocationAmt * (decimal)grantOpening.PlanReductionRate, 0, MidpointRounding.AwayFromZero))
                        + (Math.Round(grantOpening.BudgetAllocationAmt * (decimal)grantOpening.PlanSlippageRate, 0, MidpointRounding.AwayFromZero))
                        + (Math.Round(grantOpening.BudgetAllocationAmt * (decimal)grantOpening.PlanCancellationRate, 0, MidpointRounding.AwayFromZero));
        }
    }
}

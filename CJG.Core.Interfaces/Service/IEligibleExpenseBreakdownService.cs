using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IEligibleExpenseBreakdownService : IService
	{
		EligibleExpenseBreakdown Get(int id);
		IEnumerable<EligibleExpenseBreakdown> GetAllActiveForEligibleExpenseType(int id);
		IEnumerable<EligibleExpenseBreakdown> GetAllForEligibleExpenseType(int id);
		EligibleExpenseBreakdown Add(EligibleExpenseBreakdown breakdown);
		EligibleExpenseBreakdown Update(EligibleExpenseBreakdown breakdown);
		void Delete(EligibleExpenseBreakdown breakdown);
		EligibleExpenseBreakdown GetForServiceLine(int serviceLineId);
	}

}

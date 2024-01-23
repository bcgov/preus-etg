using System.Collections.Generic;
using CJG.Core.Entities;

namespace CJG.Application.Services
{
	public interface IFinanceInformationService
	{
		(int NumberOfApplications, decimal TotalPaid) GetYearToDatePaidFor(IEnumerable<GrantApplication> applications);
	}
}
using System.Collections.Generic;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IGrantApplicationJobService
	{
		IEnumerable<GrantApplication> GetUnassessedGrantApplications(int daysSinceArrival = 60);
		void ReturnUnassessed(GrantApplication grantApplication);
	}
}
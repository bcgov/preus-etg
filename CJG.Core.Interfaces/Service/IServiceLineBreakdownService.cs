using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IServiceLineBreakdownService : IService
	{
		IEnumerable<ServiceLineBreakdown> GetAll(bool isActive = true);
		IEnumerable<ServiceLineBreakdown> GetAllForServiceLine(int serviceLineId, bool isActive);
		IEnumerable<ServiceLineBreakdown> GetAllForServiceLine(int serviceLineId);
		ServiceLineBreakdown Get(int id);
		ServiceLineBreakdown Add(ServiceLineBreakdown serviceLineBreakdown);
		ServiceLineBreakdown Update(ServiceLineBreakdown serviceLineBreakdown);
		void Delete(ServiceLineBreakdown serviceLineBreakdown);
		void Remove(ServiceLineBreakdown serviceLineBreakdown);

	}
}
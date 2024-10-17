using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IServiceLineService : IService
	{
		ServiceLine Get(int id);
		IEnumerable<ServiceLine> GetAllForServiceCategory(int serviceCategoryId);
		ServiceLine Add(ServiceLine serviceCategory);
		ServiceLine Update(ServiceLine serviceCategory);
		void Delete(ServiceLine serviceCategory);
		void Remove(ServiceLine serviceLine);
	}
}
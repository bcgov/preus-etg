using CJG.Core.Entities;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IServiceCategoryService : IService
	{
		ServiceCategory Get(int id);
		IEnumerable<ServiceCategory> Get(bool? isActive = null);
		ServiceCategory Add(ServiceCategory serviceCategory);
		ServiceCategory Update(ServiceCategory serviceCategory);
		void Delete(ServiceCategory serviceCategory);
		void Remove(ServiceCategory serviceCategory);
		IEnumerable<ServiceCategory> BulkAddUpdate(IEnumerable<ServiceCategory> serviceCategories);
	}
}
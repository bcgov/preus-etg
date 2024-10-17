using System.Collections.Generic;
using CJG.Core.Entities;

namespace CJG.Core.Interfaces.Service
{
	public interface IInternalUserFilterService : IService
	{
		InternalUserFilter Get(int id);

		IEnumerable<InternalUserFilter> GetForUser();

		void Add(InternalUserFilter filter);
		void Delete(InternalUserFilter filter);
		void Update(InternalUserFilter filter);
	}
}

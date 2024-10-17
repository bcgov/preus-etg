using CJG.Core.Entities;
using CJG.Core.Entities.Helpers;
using System.Collections.Generic;

namespace CJG.Core.Interfaces.Service
{
	public interface IDenialReasonService : IService
	{
		DenialReason Get(bool? isActive);
		DenialReason Get(int id);
		void Add(DenialReason denialReason);

	}
}

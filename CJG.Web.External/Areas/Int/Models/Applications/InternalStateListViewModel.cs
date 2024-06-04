using System;
using System.Collections.Generic;
using System.Linq;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;

namespace CJG.Web.External.Areas.Int.Models
{
    public class InternalStateListViewModel : BaseViewModel
	{
		public IEnumerable<GrantApplicationInternalStateViewModel> InternalStates { get; set; }

		public InternalStateListViewModel()
		{
		}

		public InternalStateListViewModel(IEnumerable<GrantApplicationInternalState> internalStates)
		{
			if (internalStates == null)
				throw new ArgumentNullException("Internal states cannot be empty.");

			InternalStates = internalStates
				.Select(s => new GrantApplicationInternalStateViewModel(s))
				.ToArray();
		}
	}
}
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CJG.Web.External.Areas.Int.Models
{
	public class InternalStateListViewModel : BaseViewModel
	{
		#region Properties
		public IEnumerable<GrantApplicationInternalStateViewModel> InternalStates { get; set; }
		#endregion

		#region Constructors
		public InternalStateListViewModel() { }

		public InternalStateListViewModel(IEnumerable<GrantApplicationInternalState> internalStates) {
			if (internalStates == null) throw new ArgumentNullException("Internal states cannot be empty.");

			this.InternalStates = internalStates.Select(s => new GrantApplicationInternalStateViewModel(s)).ToArray();
		}
		#endregion
	}
}

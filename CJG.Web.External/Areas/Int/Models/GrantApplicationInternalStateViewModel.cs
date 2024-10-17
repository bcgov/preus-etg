using CJG.Application.Services;
using CJG.Core.Entities;
using CJG.Web.External.Models.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace CJG.Web.External.Areas.Int.Models
{
	public class GrantApplicationInternalStateViewModel : BaseViewModel
	{
		#region Properties
		public string Caption { get; set; }
		public string Description { get; set; }
		public bool IsActive { get; set; }

		#endregion

		#region Constructors
		public GrantApplicationInternalStateViewModel() { }

		public GrantApplicationInternalStateViewModel(GrantApplicationInternalState internalState)
		{
			if (internalState == null) throw new ArgumentNullException(nameof(internalState));

			Utilities.MapProperties(internalState, this);
		}
		#endregion
	}
}